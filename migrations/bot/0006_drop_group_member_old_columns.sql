--liquibase formatted sql

--changeset uamangeldiev:10
alter table bot.group_members
    drop column username,
    drop column chat_id,
    drop column is_deleted;
--rollback ;