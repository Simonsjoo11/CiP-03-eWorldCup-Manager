import { Box, CircularProgress, Typography } from '@mui/material';

interface LoadingStateProps {
  message?: string;
}

export default function LoadingState({ message = 'Loading...' }: LoadingStateProps) {
  return (
    <Box
      display="flex"
      flexDirection="column"
      alignItems="center"
      justifyContent="center"
      minHeight="200px"
      gap={2}
    >
      <CircularProgress />
      <Typography variant="body1" color="text.secondary">
        {message}
      </Typography>
    </Box>
  );
}
