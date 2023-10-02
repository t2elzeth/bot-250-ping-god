--liquibase formatted sql

--changeset uamangeldiev:10
create table bot.group_members
(
    id               bigserial,
    username         varchar(255),
    anabruhate_count bigint not null
);
--rollback ;