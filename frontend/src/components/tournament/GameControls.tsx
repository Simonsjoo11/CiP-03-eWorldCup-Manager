import { GAME_MOVES, GameMove, MOVE_EMOJIS, MOVE_LABELS } from '@/lib/constants/game';
import { Box, Button, Typography } from '@mui/material';

interface GameControlsProps {
  onMove: (move: GameMove) => void;
  disabled?: boolean;
}

export default function GameControls({ onMove, disabled = false }: GameControlsProps) {
  const moves: GameMove[] = [GAME_MOVES.ROCK, GAME_MOVES.PAPER, GAME_MOVES.SCISSORS];

  return (
    <Box>
      <Typography variant="h6" gutterBottom align="center">
        Choose Your Move
      </Typography>

      <Box display="flex" gap={2} justifyItems="center" flexWrap="wrap">
        {moves.map((move) => (
          <Button
            key={move}
            variant="contained"
            size="large"
            onClick={() => onMove(move)}
            disabled={disabled}
            sx={{
              minWidth: '120px',
              minHeight: '120px',
              fontSize: '2rem',
              flexDirection: 'column',
              gap: 1,
            }}
          >
            <Box component="span" fontSize="3rem">
              {MOVE_EMOJIS[move]}
            </Box>
            <Typography variant="button">{MOVE_LABELS[move]}</Typography>
          </Button>
        ))}
      </Box>
    </Box>
  );
}
