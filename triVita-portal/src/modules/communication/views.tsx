import { Stack, TextField } from '@mui/material';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Form, Formik } from 'formik';
import * as Yup from 'yup';
import { useState } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import { DataTable } from '@/components/common/DataTable';
import { FormikTextField } from '@/components/common/FormikTextField';
import { PageHeader } from '@/components/layout/PageHeader';
import { SectionContainer } from '@/components/ds/SectionContainer';
import { TriVitaButton } from '@/components/ds/TriVitaButton';
import {
  createNotification,
  getNotificationLogsPaged,
  getNotificationTemplatesPaged,
  sendTemplateNotification,
} from '@/services/communicationService';
import { getApiErrorMessage } from '@/utils/errorMap';
import { useToast } from '@/components/toast/ToastProvider';

const templateSendSchema = Yup.object({
  eventType: Yup.string().required(),
  templateCode: Yup.string().required(),
  channelTypeReferenceValueId: Yup.string().required().matches(/^\d+$/),
  priorityReferenceValueId: Yup.string().required().matches(/^\d+$/),
  referenceId: Yup.string().matches(/^$|^\d+$/),
  recipientTypeReferenceValueId: Yup.string().required().matches(/^\d+$/),
  email: Yup.string().max(200).nullable(),
  phoneNumber: Yup.string().max(32).nullable(),
});

const createSchema = Yup.object({
  eventType: Yup.string().required(),
  referenceId: Yup.string().matches(/^$|^\d+$/),
  priorityReferenceValueId: Yup.string().required().matches(/^\d+$/),
  statusReferenceValueId: Yup.string().matches(/^$|^\d+$/),
  recipientTypeReferenceValueId: Yup.string().required().matches(/^\d+$/),
  email: Yup.string().max(200).nullable(),
  channelTypeReferenceValueId: Yup.string().required().matches(/^\d+$/),
  templateCode: Yup.string().nullable(),
});

export function CommunicationNotificationsView() {
  const qc = useQueryClient();
  const { showToast } = useToast();
  const [tPage, setTPage] = useState(0);
  const [tSize, setTSize] = useState(20);
  const [lPage, setLPage] = useState(0);
  const [lSize, setLSize] = useState(20);
  const [tSearch, setTSearch] = useState('');
  const [tSearchApplied, setTSearchApplied] = useState('');
  const [lSearch, setLSearch] = useState('');
  const [lSearchApplied, setLSearchApplied] = useState('');

  const templates = useQuery({
    queryKey: ['comm', 'templates', tPage, tSize, tSearchApplied],
    queryFn: () => getNotificationTemplatesPaged({ page: tPage + 1, pageSize: tSize, search: tSearchApplied || undefined }),
  });
  const logs = useQuery({
    queryKey: ['comm', 'logs', lPage, lSize, lSearchApplied],
    queryFn: () => getNotificationLogsPaged({ page: lPage + 1, pageSize: lSize, search: lSearchApplied || undefined }),
  });

  const sendTpl = useMutation({
    mutationFn: sendTemplateNotification,
    onSuccess: (res) => {
      if (!res.success) showToast(res.message ?? 'Send failed', 'error');
      else showToast('Template notification queued', 'success');
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const create = useMutation({
    mutationFn: createNotification,
    onSuccess: (res) => {
      if (!res.success) showToast(res.message ?? 'Create failed', 'error');
      else {
        showToast('Notification created', 'success');
        void qc.invalidateQueries({ queryKey: ['comm'] });
      }
    },
    onError: (e) => showToast(getApiErrorMessage(e), 'error'),
  });

  const tRows =
    templates.data?.success && templates.data.data
      ? (templates.data.data.items as Record<string, unknown>[])
      : [];
  const tTotal = templates.data?.success && templates.data.data ? templates.data.data.totalCount : 0;

  const lRows =
    logs.data?.success && logs.data.data ? (logs.data.data.items as Record<string, unknown>[]) : [];
  const lTotal = logs.data?.success && logs.data.data ? logs.data.data.totalCount : 0;

  return (
    <Stack spacing={3}>
      <PageHeader
        title="Communications center"
        subtitle="Template catalogue, delivery telemetry, and guided notification composer."
        action={
          <TriVitaButton component={RouterLink} to="/communication/data-registry?resource=notifications" variant="outlined" size="small">
            API registry
          </TriVitaButton>
        }
      />

      <SectionContainer title="Send using template" subtitle="Send a templated message with structured field values.">
        <Formik
          initialValues={{
            eventType: 'PatientPortal',
            templateCode: '',
            channelTypeReferenceValueId: '',
            priorityReferenceValueId: '',
            referenceId: '',
            recipientTypeReferenceValueId: '',
            email: '',
            phoneNumber: '',
            scheduledOnUtc: '',
          }}
          validationSchema={templateSendSchema}
          onSubmit={(v) => {
            const body = {
              eventType: v.eventType.trim(),
              templateCode: v.templateCode.trim(),
              channelTypeReferenceValueId: Number(v.channelTypeReferenceValueId),
              priorityReferenceValueId: Number(v.priorityReferenceValueId),
              referenceId: v.referenceId.trim() ? Number(v.referenceId) : undefined,
              context: undefined,
              recipients: [
                {
                  recipientTypeReferenceValueId: Number(v.recipientTypeReferenceValueId),
                  recipientId: undefined,
                  email: v.email?.trim() || undefined,
                  phoneNumber: v.phoneNumber?.trim() || undefined,
                  isPrimary: true,
                },
              ],
              scheduledOnUtc: v.scheduledOnUtc ? new Date(v.scheduledOnUtc).toISOString() : undefined,
            };
            sendTpl.mutate(body);
          }}
        >
          {({ isSubmitting }) => (
            <Form>
              <Stack spacing={3} useFlexGap flexWrap="wrap" direction="row">
                <FormikTextField name="eventType" label="Event type" required sx={{ flex: '1 1 220px', minWidth: 200 }} />
                <FormikTextField name="templateCode" label="Template code" required sx={{ flex: '1 1 220px', minWidth: 200 }} />
                <FormikTextField name="channelTypeReferenceValueId" label="Channel type ref id" required sx={{ flex: '1 1 160px', minWidth: 140 }} />
                <FormikTextField name="priorityReferenceValueId" label="Priority ref id" required sx={{ flex: '1 1 160px', minWidth: 140 }} />
                <FormikTextField name="referenceId" label="Reference id (optional)" sx={{ flex: '1 1 160px', minWidth: 140 }} />
                <FormikTextField name="recipientTypeReferenceValueId" label="Recipient type ref id" required sx={{ flex: '1 1 180px', minWidth: 160 }} />
                <FormikTextField name="email" label="Recipient email" sx={{ flex: '1 1 220px', minWidth: 200 }} />
                <FormikTextField name="phoneNumber" label="Recipient phone" sx={{ flex: '1 1 180px', minWidth: 160 }} />
                <FormikTextField
                  name="scheduledOnUtc"
                  label="Schedule (local)"
                  type="datetime-local"
                  InputLabelProps={{ shrink: true }}
                  sx={{ flex: '1 1 220px', minWidth: 200 }}
                />
              </Stack>
              <TriVitaButton type="submit" variant="contained" sx={{ mt: 2 }} disabled={isSubmitting || sendTpl.isPending}>
                Send template notification
              </TriVitaButton>
            </Form>
          )}
        </Formik>
      </SectionContainer>

      <SectionContainer title="Create notification (advanced)" subtitle="Ad-hoc single-channel notification to one recipient.">
        <Formik
          initialValues={{
            eventType: 'ClinicalAlert',
            referenceId: '',
            priorityReferenceValueId: '',
            statusReferenceValueId: '',
            recipientTypeReferenceValueId: '',
            email: '',
            channelTypeReferenceValueId: '',
            templateCode: '',
          }}
          validationSchema={createSchema}
          onSubmit={(v) => {
            const body = {
              eventType: v.eventType.trim(),
              referenceId: v.referenceId.trim() ? Number(v.referenceId) : undefined,
              priorityReferenceValueId: Number(v.priorityReferenceValueId),
              statusReferenceValueId: v.statusReferenceValueId.trim() ? Number(v.statusReferenceValueId) : undefined,
              context: undefined,
              recipients: [
                {
                  recipientTypeReferenceValueId: Number(v.recipientTypeReferenceValueId),
                  email: v.email?.trim() || undefined,
                  isPrimary: true,
                },
              ],
              channels: [
                {
                  channelTypeReferenceValueId: Number(v.channelTypeReferenceValueId),
                  templateCode: v.templateCode?.trim() || undefined,
                },
              ],
              scheduledOnUtc: undefined,
            };
            create.mutate(body);
          }}
        >
          {({ isSubmitting }) => (
            <Form>
              <Stack spacing={3} useFlexGap flexWrap="wrap" direction="row">
                <FormikTextField name="eventType" label="Event type" required sx={{ flex: '1 1 220px', minWidth: 200 }} />
                <FormikTextField name="referenceId" label="Reference id (optional)" sx={{ flex: '1 1 180px', minWidth: 160 }} />
                <FormikTextField name="priorityReferenceValueId" label="Priority ref id" required sx={{ flex: '1 1 160px', minWidth: 140 }} />
                <FormikTextField name="statusReferenceValueId" label="Status ref id (optional)" sx={{ flex: '1 1 160px', minWidth: 140 }} />
                <FormikTextField name="recipientTypeReferenceValueId" label="Recipient type ref id" required sx={{ flex: '1 1 200px', minWidth: 180 }} />
                <FormikTextField name="email" label="Recipient email" sx={{ flex: '1 1 240px', minWidth: 200 }} />
                <FormikTextField name="channelTypeReferenceValueId" label="Channel type ref id" required sx={{ flex: '1 1 180px', minWidth: 160 }} />
                <FormikTextField name="templateCode" label="Template code (optional)" sx={{ flex: '1 1 200px', minWidth: 180 }} />
              </Stack>
              <TriVitaButton type="submit" variant="outlined" sx={{ mt: 2 }} disabled={isSubmitting || create.isPending}>
                Create notification
              </TriVitaButton>
            </Form>
          )}
        </Formik>
      </SectionContainer>

      <SectionContainer title="Template catalogue">
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mb: 2 }} alignItems={{ sm: 'flex-end' }}>
          <TextField
            label="Search templates"
            size="small"
            value={tSearch}
            onChange={(e) => setTSearch(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && setTSearchApplied(tSearch)}
            sx={{ flex: 1 }}
          />
          <TriVitaButton variant="outlined" onClick={() => { setTSearchApplied(tSearch); setTPage(0); }}>
            Apply
          </TriVitaButton>
        </Stack>
        <DataTable
          columns={[
            { id: 'id', label: 'ID' },
            { id: 'templateCode', label: 'Code' },
            { id: 'channelType', label: 'Channel' },
          ]}
          rows={tRows}
          rowKey={(r) => String(r.id ?? '')}
          totalCount={tTotal}
          page={tPage}
          pageSize={tSize}
          onPageChange={(p, ps) => {
            setTPage(p);
            setTSize(ps);
          }}
          loading={templates.isLoading}
        />
      </SectionContainer>

      <SectionContainer title="Delivery logs">
        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mb: 2 }} alignItems={{ sm: 'flex-end' }}>
          <TextField
            label="Search logs"
            size="small"
            value={lSearch}
            onChange={(e) => setLSearch(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && setLSearchApplied(lSearch)}
            sx={{ flex: 1 }}
          />
          <TriVitaButton variant="outlined" onClick={() => { setLSearchApplied(lSearch); setLPage(0); }}>
            Apply
          </TriVitaButton>
        </Stack>
        <DataTable
          columns={[
            { id: 'id', label: 'ID' },
            { id: 'notificationId', label: 'Notification' },
            { id: 'deliveryStatus', label: 'Status' },
          ]}
          rows={lRows}
          rowKey={(r) => String(r.id ?? '')}
          totalCount={lTotal}
          page={lPage}
          pageSize={lSize}
          onPageChange={(p, ps) => {
            setLPage(p);
            setLSize(ps);
          }}
          loading={logs.isLoading}
        />
      </SectionContainer>
    </Stack>
  );
}
