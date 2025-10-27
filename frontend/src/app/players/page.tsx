import { useGetParticipants, useGetPlayerPlayerIndexSchedule } from "@/lib/api/generated/eWorldCupApi";
import { Alert, Autocomplete, Card, CardContent, CircularProgress, Container, TextField, Typography } from "@mui/material";
import { useEffect, useState } from "react";

export default function PlayersPage() {
    const { data: partsResp, isLoading: lp, error: ep} = useGetParticipants();
    const participants = partsResp?.data?.participants ?? [];

    const [selected, setSelected] = useState<typeof participants[number] | null>(participants[0] ?? null);

    useEffect(() => {
        if (!selected && participants.length) setSelected(participants[0]);
    }, [participants, selected]);

    const idx = selected?.id ?? 0;
    const {data: schedResp, isLoading, error} = useGetPlayerPlayerIndexSchedule(idx, {
        query: {
            enabled: !!idx,
            staleTime: 5 * 60 * 1000,
            refetchOnWindowFocus: false,
        },
    });

    const schedule = schedResp?.data?.schedule ?? [];
    const playerName = schedResp?.data?.player ?? selected?.name ?? '';

    return (
        <Container maxWidth='md' sx={{py: 4}}>
            <Typography variant='h4' component='h1' gutterBottom>Spelarschema</Typography>

            {lp && <><CircularProgress size={18} /> <Typography component="span">Laddar spelare…</Typography></>}
            {ep && <Alert severity="error">Kunde inte hämta spelare.</Alert>}
        
            <Autocomplete 
                disablePortal
                options={participants}
                getOptionLabel={(o) => o.name ?? String(o.id)}
                value={selected}
                onChange={(_, val) => setSelected(val)}
                renderInput={(params) => <TextField {...params} label='Välj spelare' />}
                loading={lp}
                sx={{maxWidth: 360, my: 2}}
            />

            {isLoading && <><CircularProgress size={18} /> <Typography component="span">Laddar schema…</Typography></>}
            {error && <Alert severity="error">Kunde inte hämta schema.</Alert>}
        
            {!isLoading && !error && selected && (
                <Card>
                    <CardContent>
                        <Typography variant='h6' component='h2' gutterBottom>Schema för {playerName}</Typography>
                    </CardContent>
                </Card>
            )}
        </Container>
    )
}