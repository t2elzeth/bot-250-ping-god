--liquibase formatted sql

--changeset uamangeldiev:10
create table bot.group_member_pings
(
    id                                bigserial,
    ping                              decimal   not null,
    last_ping_date_time               timestamp not null,
    last_limit_notification_date_time timestamp null,

    constraint pk_group_member_pings primary key (id)
);
--rollback ;

--changeset uamangeldiev:20
alter table bot.group_members
    add column ping_id bigint null,
    add constraint "uq_group_members#ping" foreign key (ping_id)
        references bot.group_member_pings (id);
--rollback ;

--changeset uamangeldiev:30
alter table bot.groups
    add column allow_ping_command bool default true;
--rollback ;