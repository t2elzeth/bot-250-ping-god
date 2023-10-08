--liquibase formatted sql

--changeset uamangeldiev:10
alter table bot.group_members
    add constraint uq_group_id_member_id unique (group_id, member_id);
--rollback ;