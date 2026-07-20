-- database/schema.sql
-- Run anywhere with: psql -U postgres -d footballstats -f schema.sql
-- Or via Docker: docker exec -i <db_container_name> psql -U postgres -d footballstats -f /docker-entrypoint-initdb.d/schema.sql

BEGIN;

CREATE TABLE IF NOT EXISTS player_stats (
    id SERIAL PRIMARY KEY,
    player_id VARCHAR(255) NOT NULL,
    season VARCHAR(10) NOT NULL,
    
    -- Add your specific stat columns here based on Model/PlayerStat.cs
    -- Example mappings:
    -- goals_scored INTEGER DEFAULT 0,
    -- assists INTEGER DEFAULT 0,
    -- minutes_played INTEGER DEFAULT 0,
    -- yellow_cards INTEGER DEFAULT 0,
    -- red_cards INTEGER DEFAULT 0,
    
    created_at TIMESTAMP WITH TIME ZONE DEFAULT NOW(),
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT NOW()
);

-- Matches EF Core: modelBuilder.Entity<PlayerStat>().HasIndex(p => new { p.PlayerId, p.Season }).IsUnique();
CREATE UNIQUE INDEX IF NOT EXISTS uq_player_stats_player_season 
ON player_stats (player_id, season);

COMMIT;
