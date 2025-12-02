'use client';

import { Container, Box } from '@mui/material';
import StartTournamentForm from '@/components/tournament/StartTournamentForm';

export default function TournamentStartPage() {
  return (
    <Container maxWidth="md">
      <Box py={8}>
        <StartTournamentForm />
      </Box>
    </Container>
  );
}
