-- Postgres создаёт только POSTGRES_DB (launch_pad_catalog).
-- Остальные базы под DB-per-service создаём здесь.
-- Скрипт выполняется ОДИН раз при первой инициализации тома pg-data.
CREATE DATABASE launch_pad_ingestion;
CREATE DATABASE launch_pad_subscriptions;
CREATE DATABASE launch_pad_notifications;
