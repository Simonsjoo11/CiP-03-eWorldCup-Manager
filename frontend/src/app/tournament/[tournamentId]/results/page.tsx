'use client';

import { useParams, useRouter } from 'next/navigation';
import { Container, Box } from '@mui/material';
import { useGetTournamentTournamentIdFinal } from '@/lib/api/generated/eWorldCupApi';
import LoadingState from '@/components/ui/LoadingState';
import ErrorDisplay from '@/components/ui/ErrorDisplay';
import FinalResults from '@/components/tournament/FinalResults';

export default function TournamentResultsPage() {
  const params = useParams();
  const router = useRouter();
  const tournamentIdStr = params.tournamentId as string;
  const tournamentId = parseInt(tournamentIdStr, 10);

  const {
    data: finalData,
    isLoading,
    error,
    refetch,
  } = useGetTournamentTournamentIdFinal(tournamentId, {
    query: { enabled: !!tournamentId && !isNaN(tournamentId) },
  });

  const results = finalData?.data;

  if (isLoading) {
    return <LoadingState message="Loading results..." />;
  }

  if (error) {
    return (
      <Container maxWidth="md">
        <Box py={4}>
          <ErrorDisplay error={error} title="Failed to load results" onRetry={() => refetch()} />
        </Box>
      </Container>
    );
  }

  if (!results) {
    return null;
  }

  // Player name is in the response
  const playerName = results.playerName || 'Player';

  return (
    <Container maxWidth="lg">
      <Box py={4}>
        <FinalResults
          results={results}
          playerName={playerName}
          onNewTournament={() => router.push('/tournament')}
        />
      </Box>
    </Container>
  );
}
