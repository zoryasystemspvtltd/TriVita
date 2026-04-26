import { Autocomplete, TextField } from '@mui/material';
import { useQuery } from '@tanstack/react-query';
import { useMemo, useState } from 'react';
import { searchPatientMasters } from '@/services/hmsService';

export function PatientSearchField({
  value,
  onChange,
  error,
  helperText,
}: {
  value: { id: number; label: string } | null;
  onChange: (v: { id: number; label: string } | null) => void;
  error?: boolean;
  helperText?: string;
}) {
  const [input, setInput] = useState('');
  const q = useQuery({
    queryKey: ['pharmacy', 'patient-search', input.trim()],
    queryFn: () => searchPatientMasters({ page: 1, pageSize: 25, search: input.trim() || undefined }),
    enabled: input.trim().length >= 2,
  });

  const options = useMemo(() => {
    if (!q.data?.success || !q.data.data) return [];
    return q.data.data.items.map((p) => ({
      id: p.id,
      label: `${p.fullName}${p.primaryPhone ? ` · ${p.primaryPhone}` : ''}`,
    }));
  }, [q.data]);

  return (
    <Autocomplete
      options={options}
      loading={q.isFetching}
      value={value}
      inputValue={input}
      onInputChange={(_, v) => setInput(v)}
      onChange={(_, v) => onChange(v)}
      getOptionLabel={(o) => o.label}
      isOptionEqualToValue={(a, b) => a.id === b.id}
      renderInput={(params) => (
        <TextField
          {...params}
          label="Patient"
          required
          size="small"
          margin="normal"
          error={error}
          helperText={helperText ?? (input.trim().length > 0 && input.trim().length < 2 ? 'Type at least 2 characters' : undefined)}
        />
      )}
    />
  );
}
