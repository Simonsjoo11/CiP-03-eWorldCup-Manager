import { Box, Paper, Typography, Button } from '@mui/material';
import { FinalResultResponse } from '@/lib/types/tournament';
import Scoreboard from './Scoreboard';

interface FinalResultsProps {
  results: FinalResultResponse;
  playerName: string;
  onNewTournament: () => void;
}

export default function FinalResults({ results, playerName, onNewTournament }: FinalResultsProps) {
  const isWinner = results.playerWon;

  return (
    <Box>
      <Paper elevation={3} sx={{ p: 4, mb: 4, textAlign: 'center' }}>
        <Typography variant="h3" gutterBottom>
          Tournament Complete!
        </Typography>

        <Box my={4}>
          <Typography variant="h4" color="primary" gutterBottom>
            {results.winner || 'Unknown'}
          </Typography>
          <Typography variant="h6" color="text.secondary">
            is the champion!
          </Typography>
        </Box>

        {isWinner ? (
          <Typography variant="h5" color="success.main" sx={{ mb: 2 }}>
            Congratulations! You won the tournament!
          </Typography>
        ) : (
          <Typography variant="body1" color="text.secondary" sx={{ mb: 2 }}>
            You finished {formatOrdinal(results.playerRank)}. Better luck next time!
          </Typography>
        )}

        <Typography variant="body2" color="text.secondary">
          Total Rounds: {results.totalRounds}
        </Typography>
      </Paper>

      {results.finalScoreboard && results.finalScoreboard.length > 0 && (
        <Box mb={4}>
          <Scoreboard entries={results.finalScoreboard} highlightPlayerName={playerName} />
        </Box>
      )}

      <Box display="flex" justifyContent="center">
        <Button variant="contained" size="large" onClick={onNewTournament}>
          Start New Tournament
        </Button>
      </Box>
    </Box>
  );
}

function formatOrdinal(num: number): string {
  const suffixes = ['th', 'st', 'nd', 'rd'];
  const value = num % 100;
  return num + (suffixes[(value - 20) % 10] || suffixes[value] || suffixes[0]);
}
