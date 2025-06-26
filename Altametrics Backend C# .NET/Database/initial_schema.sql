
-- Used for checking if DB is initialized
CREATE TABLE IF NOT EXISTS DbInitialized (
    id INT PRIMARY KEY
);



-- Drop existing tables in correct dependency order
DROP TABLE IF EXISTS RSVP;
DROP TABLE IF EXISTS AuditLog;
DROP TABLE IF EXISTS Events;
DROP TABLE IF EXISTS Users;

-- USERS table
CREATE TABLE Users (
    user_id SERIAL PRIMARY KEY,
    email VARCHAR(255) UNIQUE NOT NULL,
    password_hash TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- EVENTS table
CREATE TABLE Events (
    event_id SERIAL PRIMARY KEY,
    user_id INT NOT NULL REFERENCES Users(user_id) ON DELETE CASCADE,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    event_date TIMESTAMP NOT NULL,
    location TEXT,
    event_code UUID UNIQUE NOT NULL DEFAULT gen_random_uuid(),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- RSVP table
CREATE TABLE RSVP (
    rsvp_id SERIAL PRIMARY KEY,
    event_id INT NOT NULL REFERENCES events(event_id) ON DELETE CASCADE,
    event_code UUID NOT NULL,
    guest_name VARCHAR(255),
    email VARCHAR(255) NOT NULL,
    guest_count INT DEFAULT 1,
    response_status VARCHAR(50) CHECK (response_status IN ('Going', 'Not Going', 'Maybe')),
    reminder_requested BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
-- AUDIT LOG table
CREATE TABLE AuditLog (
    audit_id SERIAL PRIMARY KEY,
    event_id INT,
    user_id INT,
    email VARCHAR(255),
    action TEXT NOT NULL,
    log_time TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Stored Procedure: audit logger
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM pg_proc WHERE proname = 'log_event_creation'
    ) THEN
        DROP PROCEDURE log_event_creation;
    END IF;
END
$$;

DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM pg_proc WHERE proname = 'log_event_creation'
    ) THEN
        DROP PROCEDURE log_event_creation;
    END IF;

    IF EXISTS (
        SELECT 1 FROM pg_proc WHERE proname = 'log_action'
    ) THEN
        DROP PROCEDURE log_action;
    END IF;
END
$$;
CREATE PROCEDURE log_action(
    p_event_id INT,
    p_user_id INT,
    p_email TEXT,
    p_action TEXT
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO AuditLog(event_id, user_id, email, action)
    VALUES (p_event_id, p_user_id, p_email, p_action);
END;
$$;

-- Stored Function: get RSVPs
CREATE OR REPLACE FUNCTION get_event_rsvp_summary(p_event_code UUID)
RETURNS TABLE(guest_name TEXT, guest_count INT, response_status TEXT)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT guest_name, guest_count, response_status
    FROM rsvp
    WHERE event_code = p_event_code;
END;
$$;

-- Indexes
CREATE INDEX idx_event_code ON Events(event_code);
CREATE INDEX idx_event_date ON Events(event_date);

-- Extension
CREATE EXTENSION IF NOT EXISTS "pgcrypto";


-- Initialize the DbInitialized table to indicate that the database has been set up
INSERT INTO DbInitialized (id) VALUES (1)
ON CONFLICT DO NOTHING;