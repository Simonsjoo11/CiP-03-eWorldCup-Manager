'use client';

import { useGetParticipants } from '@/lib/api/generated/eWorldCupApi';
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

export default function ParticipantsPage() {
  const { data, isLoading, error } = useGetParticipants({
    query: { staleTime: 30 * 60 * 1000, refetchOnWindowFocus: false },
  });
  const participants = data?.data?.participants ?? [];

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
        (participants.length ? (
          <List>
            {participants.map((p) => (
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
