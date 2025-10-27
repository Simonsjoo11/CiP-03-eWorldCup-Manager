'use client';

import { useGetParticipants, useGetRoundsMax } from '@/lib/api/generated/eWorldCupApi';

export default function Home() {
  const {
    data: partsResp,
    isLoading: lp,
    error: ep,
  } = useGetParticipants({ query: { staleTime: 30 * 60 * 1000 } });
  const {
    data: maxResp,
    isLoading: lm,
    error: em,
  } = useGetRoundsMax({}, { query: { staleTime: 30 * 60 * 1000 } });
}
