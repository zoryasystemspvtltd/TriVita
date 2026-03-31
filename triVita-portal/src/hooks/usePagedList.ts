import { useQuery, type UseQueryResult } from '@tanstack/react-query';
import { useState } from 'react';
import type { BaseResponse, PagedResponse } from '@/types/api';

export interface PagedListState<T extends object = Record<string, unknown>> {
  page: number;
  pageSize: number;
  setPage: (p: number) => void;
  setPageSize: (s: number) => void;
  onPageChange: (p: number, s: number) => void;
  rows: T[];
  totalCount: number;
  query: UseQueryResult<BaseResponse<PagedResponse<T>>, Error>;
}

/**
 * Standard paged list: React Query + 0-based UI page aligned to API 1-based `page` param.
 */
export function usePagedList<T extends object>(
  queryKeyPrefix: readonly unknown[],
  fetchPage: (apiPage: number, pageSize: number) => Promise<BaseResponse<PagedResponse<T>>>
): PagedListState<T> {
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(20);

  const query = useQuery({
    queryKey: [...queryKeyPrefix, page, pageSize],
    queryFn: () => fetchPage(page + 1, pageSize),
  });

  const pr = query.data;
  const rows =
    pr?.success && pr.data ? (pr.data.items as T[]) : [];
  const totalCount = pr?.success && pr.data ? pr.data.totalCount : 0;

  return {
    page,
    pageSize,
    setPage,
    setPageSize,
    onPageChange: (p, s) => {
      setPage(p);
      setPageSize(s);
    },
    rows,
    totalCount,
    query,
  };
}
