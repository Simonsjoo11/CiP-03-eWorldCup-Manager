'use client';

import { useGetRoundsMax, useGetRoundsRoundNumber } from '@/lib/api/generated/eWorldCupApi';
import { extractMaxRounds } from '@/lib/utils/api';
import {
  Alert,
  Box,
  Card,
  CardContent,
  CircularProgress,
  Container,
  FormControl,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  Typography,
} from '@mui/material';
import { visuallyHidden } from '@mui/utils';
import { useParams, useRouter } from 'next/navigation';
import { useEffect, useState } from 'react';

export default function RoundPage() {
  const { d } = useParams<{ d: string }>();
  const router = useRouter();

  const initial = Number(d ?? '1');
  const [round, setRound] = useState<number>(Number.isFinite(initial) && initial > 0 ? initial : 1);

  const { data: maxResp, isLoading: loadingMax, error: errorMax } = useGetRoundsMax();

  const max = extractMaxRounds(maxResp?.data);

  // Clamp invalid slug once we know max
  useEffect(() => {
    if (typeof max === 'number' && max > 0) {
      const clamped = Math.min(Math.max(round, 1), max);
      if (clamped !== round) setRound(clamped);
    }
  }, [max, round]);

  // /rounds/{roundNumber} returns AxiosResponse<RoundResponse>
  const {
    data: roundResp,
    isLoading,
    error,
  } = useGetRoundsRoundNumber(round, {
    query: { enabled: !!round },
  });

  const roundData = roundResp?.data;
  const pairs = roundData?.pairs ?? [];

  // Keep url slug in sync with Select
  useEffect(() => {
    if (!Number.isFinite(initial) || initial !== round) {
      router.replace(`/rounds/${round}`);
    }
  }, [initial, round, router]);

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        Rundvy
      </Typography>

      <Stack spacing={2}>
        <FormControl sx={{ maxWidth: 260 }} disabled={loadingMax || !max}>
          <InputLabel id="round-label">Välj runda</InputLabel>
          <Select
            labelId="round-label"
            id="round-select"
            label="Välj runda"
            value={round}
            onChange={(e) => setRound(Number(e.target.value))}
          >
            {Array.from({ length: max ?? 0 }, (_, i) => i + 1).map((r) => (
              <MenuItem key={r} value={r}>
                Runda {r}
              </MenuItem>
            ))}
          </Select>
        </FormControl>

        {/* Screen-reader live region for async status */}
        <Box role="status" aria-live="polite" sx={visuallyHidden}>
          {isLoading ? 'Laddar matcher…' : error ? 'Kunde inte hämta matcher.' : 'Matcher laddade.'}
        </Box>

        {loadingMax && (
          <Stack direction="row" alignItems="center" spacing={1}>
            <CircularProgress size={18} /> <Typography>Laddar antal rundor…</Typography>
          </Stack>
        )}
        {errorMax && <Alert severity="error">Kunde inte hämta max antal rundor.</Alert>}

        {isLoading && (
          <Stack direction="row" alignItems="center" spacing={1}>
            <CircularProgress size={18} /> <Typography>Laddar matcher…</Typography>
          </Stack>
        )}
        {error && <Alert severity="error">Kunde inte hämta matcher.</Alert>}

        {!isLoading && !error && (
          <Card>
            <CardContent>
              <Typography variant="h6" component="h2" gutterBottom>
                Matcher för runda {round}
              </Typography>

              {pairs.length ? (
                <Table aria-label={`Matcher för runda ${round}`}>
                  <TableHead>
                    <TableRow>
                      <TableCell scope="col">Hemma</TableCell>
                      <TableCell scope="col">Borta</TableCell>
                    </TableRow>
                  </TableHead>
                  <TableBody>
                    {pairs.map((m, idx) => (
                      <TableRow key={idx}>
                        <TableCell>{m.home ?? '—'}</TableCell>
                        <TableCell>{m.away ?? '—'}</TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              ) : (
                <Typography>Inga matcher hittades.</Typography>
              )}
            </CardContent>
          </Card>
        )}
      </Stack>
    </Container>
  );
}
