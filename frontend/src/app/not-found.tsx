import Link from 'next/link';
import { Container, Typography, Button, Stack } from '@mui/material';

export default function NotFound() {
  return (
    <Container maxWidth="sm" sx={{ py: 8, textAlign: 'center' }}>
      <Typography variant="h3" component="h1" gutterBottom>
        Sidan kunde inte hittas
      </Typography>
      <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
        Kontrollera adressen eller gå tillbaka till startsidan.
      </Typography>

      <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} justifyContent="center">
        <Button LinkComponent={Link} href="/" variant="contained">
          Till startsidan
        </Button>
        <Button LinkComponent={Link} href="/rounds/1" variant="outlined">
          Gå till Rundvy
        </Button>
      </Stack>
    </Container>
  );
}
