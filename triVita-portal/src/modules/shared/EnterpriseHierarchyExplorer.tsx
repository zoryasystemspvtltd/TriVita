import {
  Box,
  Collapse,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Stack,
  TextField,
  Typography,
} from '@mui/material';
import { Business, CorporateFare, Domain, ExpandLess, ExpandMore, LocalHospital, MeetingRoom } from '@mui/icons-material';
import { useQuery } from '@tanstack/react-query';
import { useEffect, useMemo, useState } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { getEnterpriseHierarchy } from '@/services/sharedService';
import { PageHeader } from '@/components/layout/PageHeader';
import { DetailDrawer } from '@/components/layout/DetailDrawer';
import { SectionContainer } from '@/components/ds/SectionContainer';
import { TriVitaButton } from '@/components/ds/TriVitaButton';
import { QueryStateBar } from '@/components/common/QueryState';

type NodeType = 'enterprise' | 'company' | 'bu' | 'facility' | 'department';

export interface HierarchyNode {
  id: string;
  type: NodeType;
  label: string;
  children: HierarchyNode[];
  payload: Record<string, unknown>;
}

function asObj(v: unknown): Record<string, unknown> {
  return v && typeof v === 'object' && !Array.isArray(v) ? (v as Record<string, unknown>) : {};
}

function buildHierarchy(raw: Record<string, unknown>): HierarchyNode[] {
  const ent = asObj(raw.enterprise ?? raw.Enterprise);
  const companies = (raw.companies ?? raw.Companies ?? []) as unknown[];
  const companyNodes: HierarchyNode[] = companies.map((wrap, ci) => {
    const w = asObj(wrap);
    const company = asObj(w.company ?? w.Company);
    const bus = (w.businessUnits ?? w.BusinessUnits ?? []) as unknown[];
    const buNodes: HierarchyNode[] = bus.map((buWrap, bi) => {
      const bw = asObj(buWrap);
      const bu = asObj(bw.businessUnit ?? bw.BusinessUnit);
      const facs = (bw.facilities ?? bw.Facilities ?? []) as unknown[];
      const fNodes: HierarchyNode[] = facs.map((fWrap, fi) => {
        const fw = asObj(fWrap);
        const fac = asObj(fw.facility ?? fw.Facility);
        const depts = (fw.departments ?? fw.Departments ?? []) as unknown[];
        const dNodes: HierarchyNode[] = depts.map((d, di) => {
          const dd = asObj(d);
          return {
            id: `dept-${String(dd.id ?? `${fi}-${di}`)}`,
            type: 'department' as const,
            label: String(dd.departmentName ?? dd.departmentCode ?? 'Department'),
            children: [],
            payload: dd,
          };
        });
        return {
          id: `fac-${String(fac.id ?? fi)}`,
          type: 'facility' as const,
          label: String(fac.facilityName ?? fac.facilityCode ?? 'Facility'),
          children: dNodes,
          payload: fac,
        };
      });
      return {
        id: `bu-${String(bu.id ?? bi)}`,
        type: 'bu' as const,
        label: String(bu.businessUnitName ?? bu.businessUnitCode ?? 'Business unit'),
        children: fNodes,
        payload: bu,
      };
    });
    return {
      id: `co-${String(company.id ?? ci)}`,
      type: 'company' as const,
      label: String(company.companyName ?? company.companyCode ?? 'Company'),
      children: buNodes,
      payload: company,
    };
  });

  return [
    {
      id: `ent-${String(ent.id ?? '1')}`,
      type: 'enterprise',
      label: String(ent.enterpriseName ?? ent.enterpriseCode ?? 'Enterprise'),
      children: companyNodes,
      payload: ent,
    },
  ];
}

function iconFor(t: NodeType) {
  switch (t) {
    case 'enterprise':
      return <Domain fontSize="small" />;
    case 'company':
      return <CorporateFare fontSize="small" />;
    case 'bu':
      return <Business fontSize="small" />;
    case 'facility':
      return <LocalHospital fontSize="small" />;
    default:
      return <MeetingRoom fontSize="small" />;
  }
}

function TreeRows({
  nodes,
  depth,
  expanded,
  toggle,
  onSelect,
}: {
  nodes: HierarchyNode[];
  depth: number;
  expanded: Set<string>;
  toggle: (id: string) => void;
  onSelect: (n: HierarchyNode) => void;
}) {
  return (
    <>
      {nodes.map((n) => {
        const hasKids = n.children.length > 0;
        const open = expanded.has(n.id);
        return (
          <Box key={n.id}>
            <ListItemButton
              onClick={() => {
                if (hasKids) toggle(n.id);
                onSelect(n);
              }}
              sx={{ pl: 1 + depth * 2 }}
            >
              <ListItemIcon sx={{ minWidth: 32 }}>
                {hasKids ? (open ? <ExpandLess /> : <ExpandMore />) : <Box sx={{ width: 24 }} />}
              </ListItemIcon>
              <ListItemIcon sx={{ minWidth: 32 }}>{iconFor(n.type)}</ListItemIcon>
              <ListItemText primary={n.label} secondary={n.type.toUpperCase()} />
            </ListItemButton>
            {hasKids ? (
              <Collapse in={open} timeout="auto" unmountOnExit>
                <List component="div" disablePadding>
                  <TreeRows
                    nodes={n.children}
                    depth={depth + 1}
                    expanded={expanded}
                    toggle={toggle}
                    onSelect={onSelect}
                  />
                </List>
              </Collapse>
            ) : null}
          </Box>
        );
      })}
    </>
  );
}

export function EnterpriseHierarchyExplorer() {
  const [enterpriseId, setEnterpriseId] = useState('1');
  const id = Number(enterpriseId) || 1;
  const q = useQuery({
    queryKey: ['shared', 'hierarchy-tree', id],
    queryFn: () => getEnterpriseHierarchy(id),
  });

  const tree = useMemo(() => {
    if (!q.data?.success || !q.data.data) return [] as HierarchyNode[];
    return buildHierarchy(q.data.data as Record<string, unknown>);
  }, [q.data]);

  const [expanded, setExpanded] = useState(() => new Set<string>());
  useEffect(() => {
    if (tree[0]?.id) setExpanded(new Set([tree[0].id]));
  }, [tree]);
  const toggle = (nodeId: string) => {
    setExpanded((prev) => {
      const next = new Set(prev);
      if (next.has(nodeId)) next.delete(nodeId);
      else next.add(nodeId);
      return next;
    });
  };

  const [selected, setSelected] = useState<HierarchyNode | null>(null);

  return (
    <Stack spacing={3}>
      <PageHeader
        title="Enterprise hierarchy"
        subtitle="Company → business unit → facility → department, exactly as returned by SharedService."
        action={
          <TriVitaButton component={RouterLink} to="/shared/data-registry?resource=enterprise-hierarchy" variant="outlined" size="small">
            API registry
          </TriVitaButton>
        }
      />

      <SectionContainer title="Load organization" subtitle="Enter the enterprise identifier you want to visualize.">
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} alignItems={{ sm: 'center' }}>
          <TextField
            label="Enterprise ID"
            size="small"
            value={enterpriseId}
            onChange={(e) => setEnterpriseId(e.target.value)}
            sx={{ width: 200 }}
          />
          <TriVitaButton variant="contained" onClick={() => void q.refetch()}>
            Refresh tree
          </TriVitaButton>
        </Stack>
      </SectionContainer>

      <QueryStateBar isLoading={q.isFetching} isError={q.isError} error={q.error} />
      {q.data && q.data.success === false ? (
        <Typography color="error">{q.data.message ?? 'Unable to load hierarchy'}</Typography>
      ) : null}

      <SectionContainer title="Directory tree" subtitle="Expand nodes to walk the chain; select any node to inspect DTO fields.">
        <List dense sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 2, maxHeight: 560, overflow: 'auto' }}>
          {tree.length ? (
            <TreeRows nodes={tree} depth={0} expanded={expanded} toggle={toggle} onSelect={setSelected} />
          ) : (
            <Typography color="text.secondary" sx={{ p: 2 }}>
              No data — verify enterprise ID and permissions.
            </Typography>
          )}
        </List>
      </SectionContainer>

      <DetailDrawer
        open={Boolean(selected)}
        onClose={() => setSelected(null)}
        title={selected?.label ?? ''}
        subtitle={selected ? selected.type.toUpperCase() : undefined}
      >
        {selected ? (
          <Stack spacing={1.5}>
            {Object.entries(selected.payload).map(([k, v]) => (
              <Box key={k}>
                <Typography variant="caption" color="text.secondary" display="block">
                  {k}
                </Typography>
                <Typography variant="body2">{v === null || v === undefined ? '—' : String(v)}</Typography>
              </Box>
            ))}
          </Stack>
        ) : null}
      </DetailDrawer>
    </Stack>
  );
}
