'use client';
import { VisuallyHidden } from "@chakra-ui/react";
import {Box, Heading, Spinner, Text} from '@chakra-ui/react';
import { useGetParticipants } from "@/lib/api/generated/eWorldCupApi";
import type { ParticipantsResponse } from "@/lib/api/generated/model";

export default function ParticipantsPage() {
    const {data, isLoading, error} = useGetParticipants();

    const payload = (data?.data ?? {}) as ParticipantsResponse;
    const participants = payload.participants ?? [];

    return (
        <Box>
            <Heading>
                Participants
            </Heading>
            <VisuallyHidden role="status" aria-live="polite">
                {isLoading ? 'Loading participants…' : error ? 'Failed to load participants.' : 'Participants loaded.'}
            </VisuallyHidden>

            {isLoading && (
                <Box display="flex" gap={2} alignItems="center">
                <Spinner /> <Text>Loading participants…</Text>
                </Box>
            )}

            {error && (
                <Box role="alert" mt={2} color="red.600">
                Couldn’t load participants. Please try again.
                </Box>
            )}

            {!isLoading && !error && (
                participants.length ? (
                <Box as="ul">
                    {participants.map((p) => (
                    <Box as="li" key={p.id}>
                        <Text as="span">{p.name}</Text>
                    </Box>
                    ))}
                </Box>
                ) : (
                <Text>No participants found.</Text>
                )
            )}
        </Box>
    )
}