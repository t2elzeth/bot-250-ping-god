create user t2elzeth with password '123';

create database bot_250_ping_god_db with owner t2elzeth;

\connect bot_250_ping_god_db;
create schema liquibase authorization t2elzeth;