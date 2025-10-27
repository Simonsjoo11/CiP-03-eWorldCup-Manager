import Link from 'next/link';
import { AppBar, Toolbar, Container, Typography, Stack, Button, Box } from '@mui/material';

export default function SiteChrome({ children }: { children: React.ReactNode }) {
  return (
    <>
      <AppBar position="static" color="default" elevation={0}>
        <Toolbar>
          <Typography
            variant="h6"
            component={Link}
            href="/"
            sx={{ flexGrow: 1, textDecoration: 'none', color: 'inherit', fontWeight: 700 }}
          >
            E-WorldCup
          </Typography>

          <Stack direction="row" spacing={1}>
            <Button LinkComponent={Link} href="/rounds/1">
              Rundor
            </Button>
            <Button LinkComponent={Link} href="/players" variant="outlined">
              Spelarschema
            </Button>
            <Button LinkComponent={Link} href="/participants" variant="text">
              Deltagare
            </Button>
          </Stack>
        </Toolbar>
      </AppBar>

      <Container component="main" id="main" maxWidth="lg" sx={{ py: 3 }}>
        {children}
      </Container>

      <Box component="footer" sx={{ borderTop: 1, borderColor: 'divider', mt: 6, py: 2 }}>
        <Container maxWidth="lg">
          <Typography variant="body2">© {new Date().getFullYear()} E-WorldCup</Typography>
        </Container>
      </Box>
    </>
  );
}
