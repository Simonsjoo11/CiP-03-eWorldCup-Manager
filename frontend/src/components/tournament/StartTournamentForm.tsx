'use client';

import { usePostTournamentStart } from '@/lib/api/generated/eWorldCupApi';
import { useRouter } from 'next/navigation';
import { useState } from 'react';
import LoadingState from '../ui/LoadingState';
import {
  Alert,
  Box,
  Button,
  FormControl,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  TextField,
  Typography,
} from '@mui/material';

const PLAYER_OPTIONS = [4, 6, 8, 10, 12, 16];

export default function StartTournamentForm() {
  const [name, setName] = useState('');
  const [playerCount, setPlayerCount] = useState(8);
  const router = useRouter();

  const {
    mutate: startTournament,
    isPending,
    error,
  } = usePostTournamentStart({
    mutation: {
      onSuccess: (response) => {
        router.push(`/tournament/${response.data.tournamentId}`);
      },
    },
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (!name.trim()) return;

    startTournament({ data: { playerName: name.trim(), totalPlayers: playerCount } });
  };

  if (isPending) {
    return <LoadingState message="Starting tournament..." />;
  }

  return (
    <Box maxWidth="500px" mx="auto">
      <Paper elevation={3} sx={{ p: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom align="center">
          Rock Paper Arena
        </Typography>
        <Typography variant="body1" component="p" color="text.secondary" align="center">
          Start a new tournament and compete against AI opponents!
        </Typography>

        <form onSubmit={handleSubmit}>
          <Box display="flex" flexDirection="column" gap={3} mt={3}>
            <TextField
              label="Your name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
              fullWidth
              autoFocus
              placeholder="Enter your name"
            />

            <FormControl fullWidth>
              <InputLabel>Number of Players</InputLabel>
              <Select
                value={playerCount}
                label="Number of Players"
                onChange={(e) => setPlayerCount(e.target.value as number)}
              >
                {PLAYER_OPTIONS.map((count) => (
                  <MenuItem key={count} value={count}>
                    {count} Players
                  </MenuItem>
                ))}
              </Select>
            </FormControl>

            {error && (
              <Alert severity="error">
                <>{error.response?.data?.message || 'Failed to start tournament'}</>
              </Alert>
            )}

            <Button
              type="submit"
              variant="contained"
              size="large"
              fullWidth
              disabled={!name.trim() || isPending}
            >
              Start Tournament
            </Button>
          </Box>
        </form>
      </Paper>
    </Box>
  );
}
