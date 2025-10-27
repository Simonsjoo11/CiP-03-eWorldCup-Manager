'use client';

import { useGetParticipants, useGetRoundsMax } from '@/lib/api/generated/eWorldCupApi';
import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  CircularProgress,
  Container,
  Stack,
  Typography,
} from '@mui/material';
import Link from 'next/link';

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

  const n = partsResp?.data?.participants?.length ?? 0;
  const rawMax = maxResp?.data;
  const max =
    typeof rawMax === 'number'
      ? rawMax
      : rawMax && typeof rawMax === 'object' && 'max' in (rawMax as any)
        ? (rawMax as any).max
        : undefined;

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        E-WorldCup Manager
      </Typography>

      {(lp || lm) && (
        <Stack direction="row" spacing={1} alignItems="center">
          <CircularProgress size={18} />
          <Typography>Laddar...</Typography>
        </Stack>
      )}

      {(ep || em) && <Alert severity="error">Kunde inte hämta information.</Alert>}

      {!lp && !lm && !ep && !em && (
        <Stack spacing={2}>
          <Card>
            <CardContent>
              <Typography>
                <strong>Antal deltagare (n):</strong> {n}
              </Typography>
              <Typography>
                <strong>Max antal rundor:</strong> {max ?? '—'}
              </Typography>
            </CardContent>
          </Card>

          <Box role="navigation" aria-label="Huvudnavigering">
            <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2}>
              <Button LinkComponent={Link} href="/rounds/1" variant="contained">
                Gå till Rundvy
              </Button>
              <Button LinkComponent={Link} href="/players" variant="outlined">
                Gå till Spelarschema
              </Button>
              <Button LinkComponent={Link} href="/participants" variant="text">
                Visa deltagare
              </Button>
            </Stack>
          </Box>
        </Stack>
      )}
    </Container>
  );
}
