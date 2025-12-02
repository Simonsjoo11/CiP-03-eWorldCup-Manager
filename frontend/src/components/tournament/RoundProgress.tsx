import { GAME_CONFIG } from '@/lib/constants/game';
import { MatchStatusDto } from '@/lib/types/tournament';
import { Box, Chip, Typography } from '@mui/material';

interface RoundProgressProps {
  match: MatchStatusDto;
}

export default function RoundProgress({ match }: RoundProgressProps) {
  const currentRound = match.gameRoundNumber || 1;
  const maxRounds = 3;
  const playerWins = match.playerWins || 0;
  const opponentWins = match.opponentWins || 0;

  return (
    <Box>
      <Typography variant="h6" gutterBottom>
        Match Progress (Best of {maxRounds})
      </Typography>

      <Box display="flex" alignItems="center" gap={2} mb={2}>
        <Chip
          label={`You: ${playerWins}`}
          color={playerWins >= GAME_CONFIG.ROUNDS_TO_WIN ? 'success' : 'default'}
          variant={playerWins > opponentWins ? 'filled' : 'outlined'}
        />
        <Typography variant="body2" color="text.secondary">
          -
        </Typography>
        <Chip
          label={`${match.opponentName || 'Opponent'}: ${opponentWins}`}
          color={opponentWins >= GAME_CONFIG.ROUNDS_TO_WIN ? 'error' : 'default'}
          variant={opponentWins > playerWins ? 'filled' : 'outlined'}
        />
      </Box>

      <Typography variant="body2" color="text.secondary">
        Round {currentRound} of {maxRounds}
      </Typography>

      <Box display="flex" gap={1} mt={1}>
        {Array.from({ length: maxRounds }, (_, i) => i + 1).map((round) => (
          <Box
            key={round}
            sx={{
              width: 12,
              height: 12,
              borderRadius: '50%',
              backgroundColor: round < currentRound ? 'primary.main' : 'grey.300',
            }}
          />
        ))}
      </Box>
    </Box>
  );
}
