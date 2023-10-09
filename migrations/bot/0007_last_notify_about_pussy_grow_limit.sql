--liquibase formatted sql

--changeset uamangeldiev:10
alter table bot.group_member_pussies
    add column last_limit_notification_date_time timestamp null;
--rollback ;