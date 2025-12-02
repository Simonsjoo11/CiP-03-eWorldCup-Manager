'use client';

import { useGetPlayer } from '@/lib/api/generated/eWorldCupApi';
import {
  Alert,
  CircularProgress,
  Container,
  List,
  ListItem,
  ListItemText,
  Stack,
  Typography,
} from '@mui/material';

export default function PlayersPage() {
  const { data, isLoading, error } = useGetPlayer({
    query: { staleTime: 30 * 60 * 1000, refetchOnWindowFocus: false },
  });
  const players = data?.data?.players ?? [];

  return (
    <Container maxWidth="sm" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        Deltagare
      </Typography>

      {isLoading && (
        <Stack direction="row" spacing={1} alignItems="center">
          <CircularProgress size={18} /> <Typography>Laddar deltagare...</Typography>
        </Stack>
      )}
      {error && <Alert severity="error">Kunde inte hämta deltagare.</Alert>}

      {!isLoading &&
        !error &&
        (players.length ? (
          <List>
            {players.map((p) => (
              <ListItem key={p.id} divider>
                <ListItemText primary={p.name ?? `#${p.id}`} />
              </ListItem>
            ))}
          </List>
        ) : (
          <Typography>Inga deltagare.</Typography>
        ))}
    </Container>
  );
}
