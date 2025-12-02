export const GAME_MOVES = {
  ROCK: 'rock',
  PAPER: 'paper',
  SCISSORS: 'scissors',
} as const;

export type GameMove = (typeof GAME_MOVES)[keyof typeof GAME_MOVES];

export const GAME_CONFIG = {
  ROUNDS_TO_WIN: 2,
  MAX_ROUNDS: 3,
  POINTS: {
    WIN: 3,
    DRAW: 1,
    LOSS: 0,
  },
} as const;

export const MOVE_EMOJIS: Record<GameMove, string> = {
  rock: '✊',
  paper: '✋',
  scissors: '✌️',
};

export const MOVE_LABELS: Record<GameMove, string> = {
  rock: 'Rock',
  paper: 'Paper',
  scissors: 'Scissors',
};

export const stringToRpsChoice = (move: GameMove): number => {
  const mapping = {
    [GAME_MOVES.ROCK]: 0,
    [GAME_MOVES.PAPER]: 1,
    [GAME_MOVES.SCISSORS]: 2,
  };
  return mapping[move];
};

export const rpsChoiceToString = (choice: number): GameMove => {
  const moves = [GAME_MOVES.ROCK, GAME_MOVES.PAPER, GAME_MOVES.SCISSORS];
  return moves[choice];
};
