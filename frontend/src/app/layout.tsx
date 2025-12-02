import type { Metadata } from 'next';
import QueryProvider from '@/providers/QueryProvider';
import { AppRouterCacheProvider } from '@mui/material-nextjs/v15-appRouter';
import theme from '@/theme';
import { ThemeProvider } from '@mui/material/styles';
import { Roboto } from 'next/font/google';
import SiteChrome from '@/components/SiteChrome';

const roboto = Roboto({
  weight: ['300', '400', '500', '700'],
  subsets: ['latin'],
  display: 'swap',
  variable: '--font-roboto',
});

export const metadata: Metadata = {
  title: 'E-WorldCup Manager',
  description: 'Round view, player schedules and players',
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" className={roboto.variable}>
      <body>
        <AppRouterCacheProvider>
          <ThemeProvider theme={theme}>
            <QueryProvider>
              <SiteChrome>{children}</SiteChrome>
            </QueryProvider>
          </ThemeProvider>
        </AppRouterCacheProvider>
      </body>
    </html>
  );
}
