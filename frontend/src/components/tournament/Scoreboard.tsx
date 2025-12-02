import {
  Paper,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Box,
} from '@mui/material';
import { ScoreboardEntryDto } from '@/lib/types/tournament';
import { formatOrdinal } from '@/lib/utils/format';

interface ScoreboardProps {
  entries: ScoreboardEntryDto[];
  highlightPlayerName?: string;
}

export default function Scoreboard({ entries, highlightPlayerName }: ScoreboardProps) {
  // Calculate matchesPlayed from wins + losses
  const getMatchesPlayed = (entry: ScoreboardEntryDto) => entry.wins + entry.losses;

  return (
    <Paper elevation={2}>
      <Box p={2}>
        <Typography variant="h5" gutterBottom>
          Scoreboard
        </Typography>
      </Box>

      <TableContainer>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Rank</TableCell>
              <TableCell>Player</TableCell>
              <TableCell align="center">Played</TableCell>
              <TableCell align="center">W</TableCell>
              <TableCell align="center">L</TableCell>
              <TableCell align="right">Points</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {entries.map((entry, index) => {
              const isHighlighted = entry.playerName === highlightPlayerName;
              return (
                <TableRow
                  key={entry.playerIndex}
                  sx={{
                    backgroundColor: isHighlighted ? 'action.selected' : 'inherit',
                    fontWeight: isHighlighted ? 'bold' : 'normal',
                  }}
                >
                  <TableCell>{formatOrdinal(index + 1)}</TableCell>
                  <TableCell>
                    <strong>{entry.playerName}</strong>
                    {isHighlighted && ' (You)'}
                  </TableCell>
                  <TableCell align="center">{getMatchesPlayed(entry)}</TableCell>
                  <TableCell align="center">{entry.wins}</TableCell>
                  <TableCell align="center">{entry.losses}</TableCell>
                  <TableCell align="right">
                    <strong>{entry.points}</strong>
                  </TableCell>
                </TableRow>
              );
            })}
          </TableBody>
        </Table>
      </TableContainer>
    </Paper>
  );
}
