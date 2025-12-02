import { Box, Paper, Typography, Alert, Chip } from '@mui/material';
import { MatchStatusDto, PlayRoundResponse } from '@/lib/types/tournament';
import { MOVE_EMOJIS, MOVE_LABELS, rpsChoiceToString } from '@/lib/constants/game';

interface MatchStatusProps {
  match: MatchStatusDto;
  lastResult?: PlayRoundResponse | null;
}

export default function MatchStatus({ match, lastResult }: MatchStatusProps) {
  const playerWins = match.playerWins || 0;
  const opponentWins = match.opponentWins || 0;

  const getStatusMessage = () => {
    if (playerWins === 2) return 'You won the match!';
    if (opponentWins === 2) return 'You lost the match!';
    if (playerWins > opponentWins) return "You're leading!";
    if (opponentWins > playerWins) return "You're behind!";
    return "It's tied!";
  };

  const getAlertSeverity = (result: string | null) => {
    if (!result) return 'info';
    if (result === 'Player1Win') return 'success';
    if (result === 'Player2Win') return 'error';
    return 'info'; // Draw
  };

  return (
    <Paper elevation={2} sx={{ p: 3 }}>
      <Typography variant="h5" gutterBottom>
        Current Match
      </Typography>

      <Box display="flex" alignItems="center" gap={2} mb={2}>
        <Typography variant="h6">You</Typography>
        <Typography variant="body2" color="text.secondary">
          vs
        </Typography>
        <Typography variant="h6">{match.opponentName || 'Opponent'}</Typography>
      </Box>

      <Chip
        label={getStatusMessage()}
        color={
          playerWins > opponentWins ? 'success' : opponentWins > playerWins ? 'error' : 'default'
        }
      />

      {lastResult && (
        <Alert severity={getAlertSeverity(lastResult.result)} sx={{ mt: 2 }}>
          <Box display="flex" alignItems="center" gap={2} mb={1}>
            <Typography variant="body2">
              You: {MOVE_EMOJIS[rpsChoiceToString(lastResult.playerChoice)]}{' '}
              {MOVE_LABELS[rpsChoiceToString(lastResult.playerChoice)]}
            </Typography>
            <Typography variant="body2">
              {match.opponentName}: {MOVE_EMOJIS[rpsChoiceToString(lastResult.opponentChoice)]}{' '}
              {MOVE_LABELS[rpsChoiceToString(lastResult.opponentChoice)]}
            </Typography>
          </Box>
          <Typography variant="body2">{lastResult.message}</Typography>
        </Alert>
      )}
    </Paper>
  );
}
