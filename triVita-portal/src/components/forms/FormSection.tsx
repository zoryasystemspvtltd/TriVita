import { SectionContainer, type SectionContainerProps } from '@/components/ds/SectionContainer';

/** Form-friendly alias for `SectionContainer` (16px card spacing, grouped fields). */
export function FormSection(props: SectionContainerProps) {
  return <SectionContainer {...props} />;
}
