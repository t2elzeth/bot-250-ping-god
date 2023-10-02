--liquibase formatted sql

--changeset uamangeldiev:10
create table bot.anabruhated_users
(
    id       bigserial,
    username varchar(255)
);
--rollback ;