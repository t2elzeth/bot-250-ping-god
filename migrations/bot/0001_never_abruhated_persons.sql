--liquibase formatted sql

--changeset uamangeldiev:10
create table bot.never_abruhated_persons
(
    id       bigserial,
    username varchar(255)
);
--rollback ;