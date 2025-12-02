'use client';

import { useState, useEffect } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { Container, Box, Button, Typography, Alert, Stack } from '@mui/material';
import {
  useGetTournamentTournamentIdStatus,
  usePostTournamentTournamentIdPlay,
  usePostTournamentTournamentIdAdvance,
} from '@/lib/api/generated/eWorldCupApi';
import { stringToRpsChoice, GameMove } from '@/lib/constants/game';
import { RpsChoice } from '@/lib/api/generated/model';
import LoadingState from '@/components/ui/LoadingState';
import ErrorDisplay from '@/components/ui/ErrorDisplay';
import GameControls from '@/components/tournament/GameControls';
import MatchStatus from '@/components/tournament/MatchStatus';
import RoundProgress from '@/components/tournament/RoundProgress';
import Scoreboard from '@/components/tournament/Scoreboard';
import { PlayRoundResponse } from '@/lib/types/tournament';

export default function TournamentPlayPage() {
  const params = useParams();
  const router = useRouter();
  const tournamentIdStr = params.tournamentId as string;
  const tournamentId = parseInt(tournamentIdStr, 10);

  const [showScoreboard, setShowScoreboard] = useState(false);
  const [lastResult, setLastResult] = useState<PlayRoundResponse | null>(null);
  const [playerName, setPlayerName] = useState<string>('');

  // Fetch tournament status
  const {
    data: statusData,
    isLoading,
    error,
    refetch,
  } = useGetTournamentTournamentIdStatus(tournamentId, {
    query: {
      enabled: !!tournamentId && !isNaN(tournamentId),
      refetchInterval: showScoreboard ? 5000 : false,
    },
  });

  const status = statusData?.data;

  // Store player name from first load
  useEffect(() => {
    if (status?.scoreboard && status.scoreboard.length > 0 && !playerName) {
      const player = status.scoreboard.find((s) => s.playerIndex === 0);
      if (player?.playerName) {
        setPlayerName(player.playerName);
      }
    }
  }, [status?.scoreboard, playerName]);

  // Play move mutation
  const playMove = usePostTournamentTournamentIdPlay({
    mutation: {
      onSuccess: (response) => {
        setLastResult(response.data);
        if (response.data.matchComplete) {
          setShowScoreboard(true);
        }
        refetch();
      },
    },
  });

  // Advance tournament mutation
  const advance = usePostTournamentTournamentIdAdvance({
    mutation: {
      onSuccess: () => {
        setShowScoreboard(false);
        setLastResult(null);
        refetch();
      },
    },
  });

  // Redirect to results when tournament complete
  useEffect(() => {
    if (status?.status === 'Completed') {
      router.push(`/tournament/${tournamentIdStr}/results`);
    }
  }, [status?.status, router, tournamentIdStr]);

  const handleMove = (move: GameMove) => {
    const choice = stringToRpsChoice(move) as RpsChoice;
    playMove.mutate({
      tournamentId,
      data: { choice },
    });
  };

  const handleAdvance = () => {
    advance.mutate({
      tournamentId,
    });
  };

  if (isLoading) {
    return <LoadingState message="Loading tournament..." />;
  }

  if (error) {
    return (
      <Container maxWidth="md">
        <Box py={4}>
          <ErrorDisplay error={error} title="Failed to load tournament" onRetry={() => refetch()} />
        </Box>
      </Container>
    );
  }

  if (!status) {
    return null;
  }

  const currentMatch = status.currentMatch;
  const canPlayMove = currentMatch && !showScoreboard && !playMove.isPending;

  return (
    <Container maxWidth="lg">
      <Box py={4}>
        {/* Header */}
        <Box mb={4} textAlign="center">
          <Typography variant="h3" gutterBottom>
            Rock Paper Arena
          </Typography>
          <Typography variant="h6" color="text.secondary">
            Round {status.currentRound} of {status.maxRounds}
          </Typography>
        </Box>

        {/* Game or Scoreboard View */}
        {!showScoreboard && currentMatch ? (
          <Stack spacing={4}>
            <MatchStatus match={currentMatch} lastResult={lastResult} />
            <RoundProgress match={currentMatch} />
            <GameControls onMove={handleMove} disabled={!canPlayMove} />

            {playMove.isPending && <LoadingState message="Processing your move..." />}

            {playMove.error && (
              <ErrorDisplay
                error={playMove.error}
                title="Failed to play move"
                onRetry={() => playMove.reset()}
              />
            )}

            {lastResult?.matchComplete && (
              <Alert severity={lastResult.matchWinner === playerName ? 'success' : 'error'}>
                <Typography variant="h6">
                  {lastResult.matchWinner === playerName
                    ? 'You won the match!'
                    : 'You lost the match!'}
                </Typography>
                <Typography variant="body2">Click below to view the scoreboard.</Typography>
              </Alert>
            )}

            {lastResult?.matchComplete && (
              <Box display="flex" justifyContent="center">
                <Button variant="outlined" size="large" onClick={() => setShowScoreboard(true)}>
                  View Scoreboard
                </Button>
              </Box>
            )}
          </Stack>
        ) : (
          <Stack spacing={4}>
            <Scoreboard entries={status.scoreboard || []} highlightPlayerName={playerName} />

            <Box display="flex" justifyContent="center">
              <Button
                variant="contained"
                size="large"
                onClick={handleAdvance}
                disabled={advance.isPending}
              >
                {advance.isPending ? 'Advancing...' : 'Next Round'}
              </Button>
            </Box>

            {advance.error && (
              <ErrorDisplay
                error={advance.error}
                title="Failed to advance"
                onRetry={() => advance.reset()}
              />
            )}
          </Stack>
        )}
      </Box>
    </Container>
  );
}
