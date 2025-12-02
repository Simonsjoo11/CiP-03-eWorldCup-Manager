'use client';

import { useGetPlayer, useGetPlayerPlayerIndexSchedule } from '@/lib/api/generated/eWorldCupApi';
import {
  Alert,
  Autocomplete,
  Card,
  CardContent,
  CircularProgress,
  Container,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography,
  Link as MuiLink,
} from '@mui/material';
import Link from 'next/link';
import { useEffect, useState } from 'react';

export default function PlayersPage() {
  const { data: partsResp, isLoading: lp, error: ep } = useGetPlayer();
  const players = partsResp?.data?.players ?? [];

  const [selected, setSelected] = useState<(typeof players)[number] | null>(null);
  useEffect(() => {
    if (!selected && players.length) setSelected(players[0]);
  }, [players, selected]);

  const selectedIndex = selected ? players.findIndex((p) => p.id === selected.id) : -1;

  const {
    data: schedResp,
    isLoading,
    error,
  } = useGetPlayerPlayerIndexSchedule(selectedIndex >= 0 ? selectedIndex : 0, {
    query: {
      enabled: selectedIndex >= 0,
      staleTime: 5 * 60 * 1000,
      refetchOnWindowFocus: false,
    },
  });

  const schedule = schedResp?.data?.schedule ?? [];
  const playerName = schedResp?.data?.player ?? selected?.name ?? '';

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        Spelarschema
      </Typography>

      {lp && (
        <>
          <CircularProgress size={18} /> <Typography component="span">Laddar spelare…</Typography>
        </>
      )}
      {ep && <Alert severity="error">Kunde inte hämta spelare.</Alert>}

      <Autocomplete
        disablePortal
        options={players}
        value={selected}
        onChange={(_, val) => setSelected(val)}
        isOptionEqualToValue={(option, value) => option.id === value.id}
        getOptionLabel={(o) => o?.name ?? String(o?.id ?? '')}
        renderInput={(params) => <TextField {...params} label="Välj spelare" />}
        loading={lp}
        sx={{ maxWidth: 360, my: 2 }}
        noOptionsText={lp ? 'Laddar…' : 'Inga spelare'}
      />

      {isLoading && (
        <>
          <CircularProgress size={18} /> <Typography component="span">Laddar schema…</Typography>
        </>
      )}
      {error && <Alert severity="error">Kunde inte hämta schema.</Alert>}

      {!isLoading && !error && selected && (
        <Card>
          <CardContent>
            <Typography variant="h6" component="h2" gutterBottom>
              Schema för {playerName}
            </Typography>
            {schedule.length ? (
              <Table aria-label={`Schema för ${playerName}`}>
                <TableHead>
                  <TableRow>
                    <TableCell scope="col">Runda</TableCell>
                    <TableCell scope="col">Motståndare</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {schedule.map((s, i) => (
                    <TableRow key={i}>
                      <TableCell>
                        <MuiLink component={Link} href={`/rounds/${s.round}`} underline="always">
                          {s.round}
                        </MuiLink>
                      </TableCell>
                      <TableCell>{s.opponent ?? `#${s.opponentIndex}`}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            ) : (
              <Typography>Inga rader.</Typography>
            )}
          </CardContent>
        </Card>
      )}
    </Container>
  );
}
