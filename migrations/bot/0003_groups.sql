--liquibase formatted sql

--changeset uamangeldiev:10
create table bot.groups
(
    id      bigserial    not null,
    title   varchar(255) null,
    chat_id bigint       not null,

    constraint pk_groups primary key (id)
);
--rollback ;

--changeset uamangeldiev:20
alter table bot.group_members
    add column group_id bigint null,
    add constraint "fk_group_member#group" foreign key (group_id)
        references bot.groups (id);
--rollback ;