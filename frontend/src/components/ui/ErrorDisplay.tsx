import { Alert, AlertTitle, Box, Button } from '@mui/material';
import { AxiosError } from 'axios';

interface ApiError {
  message?: string;
}

interface ErrorDisplayProps {
  error: AxiosError<ApiError> | Error;
  title?: string;
  onRetry?: () => void;
}

export default function ErrorDisplay({ error, title = 'Error', onRetry }: ErrorDisplayProps) {
  const message =
    error instanceof AxiosError ? error.response?.data?.message || error.message : error.message;

  return (
    <Box py={2}>
      <Alert severity="error">
        <AlertTitle>{title}</AlertTitle>
        {message}
        {onRetry && (
          <Box mt={2}>
            <Button variant="outlined" size="small" onClick={onRetry}>
              Try Again
            </Button>
          </Box>
        )}
      </Alert>
    </Box>
  );
}
