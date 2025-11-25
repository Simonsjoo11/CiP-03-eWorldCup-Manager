import { GameMove } from '../constants/game';

// Tournament types
export interface TournamentStartRequest {
  name: string;
  players: number;
}

export interface TournamentStartResponse {
  tournamentId: string;
  currentRound: number;
  totalROunds: number;
  currentOpponent: PlayerInfo;
  message: string;
}

export interface PlayerInfo {
  id: number;
  name: string;
}

// Status types
export interface TournamentStatusResponse {
  tournamentId: string;
  currentRound: number;
  totalRounds: number;
  currentMatch: CurrentMatch | null;
  scoreboard: ScoreboardEntry[];
  isComplete: boolean;
  allMatchesInRoundComplete: boolean;
}

export interface CurrentMatch {
  opponent: PlayerInfo;
  roundNumber: number;
  maxRounds: number;
  playerWins: number;
  opponentWins: number;
  lastResult?: RoundResult;
}

export interface RoundResult {
  playerMove: GameMove;
  opponentMove: GameMove;
  outcome: 'win' | 'loss' | 'draw';
  message: string;
}

// Play types
export interface PlayMoveRequest {
  move: GameMove;
}

export interface PlayMoveResponse {
  roundNumber: number;
  maxRounds: number;
  playerWins: number;
  opponentWins: number;
  playerMove: GameMove;
  opponentMove: GameMove;
  outcome: 'win' | 'loss' | 'draw';
  message: string;
  matchComplete: boolean;
  matchOutcome?: 'win' | 'loss';
}

// Scoreboard types
export interface ScoreboardEntry {
  rank: number;
  player: PlayerInfo;
  played: number;
  wins: number;
  draws: number;
  losses: number;
  points: number;
}

// Final results types
export interface TournamentFinalResponse {
  tournamentId: string;
  winner: PlayerInfo;
  scoreboard: ScoreboardEntry[];
  totalRounds: number;
  message: string;
}

export interface ApiError {
  message: string;
  status?: number;
}
