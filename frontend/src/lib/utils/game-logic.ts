import { GAME_MOVES, GameMove } from '../constants/game';

export const determineWinner = (
  playerMove: GameMove,
  opponentMove: GameMove,
): 'win' | 'loss' | 'draw' => {
  if (playerMove === opponentMove) return 'draw';

  const winConditions: Record<GameMove, GameMove> = {
    [GAME_MOVES.ROCK]: GAME_MOVES.SCISSORS,
    [GAME_MOVES.PAPER]: GAME_MOVES.ROCK,
    [GAME_MOVES.SCISSORS]: GAME_MOVES.PAPER,
  };

  return winConditions[playerMove] === opponentMove ? 'win' : 'loss';
};

export const getOutcomeMessage = (
  outcome: 'win' | 'loss' | 'draw',
  playerMove: GameMove,
  opponentMove: GameMove,
): string => {
  if (outcome === 'draw') {
    return `Both chose ${playerMove}. It's a draw!`;
  }

  if (outcome === 'win') {
    return `${playerMove} beats ${opponentMove}. You win this round!`;
  }

  return `${opponentMove} beats ${playerMove}. You lose this round!`;
};

export const getMatchStatusMessage = (playerWins: number, opponentWins: number): string => {
  if (playerWins === 2) return 'You won the match!';
  if (opponentWins === 2) return 'You lost the match!';

  if (playerWins > opponentWins) return "You're leading!";
  if (opponentWins > playerWins) return "You're behind!";
  return "It's tied!";
};
