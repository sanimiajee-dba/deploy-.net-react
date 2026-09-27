-- Run this on your Oracle 19c instance (192.168.0.231) as a DBA user
-- to prepare the demo schema and sample data.

-- 1. Create a demo user/schema (adjust password!)
CREATE USER demo_user IDENTIFIED BY demo_pass
  DEFAULT TABLESPACE users
  TEMPORARY TABLESPACE temp
  QUOTA UNLIMITED ON users;

GRANT CREATE SESSION, CREATE TABLE, CREATE SEQUENCE TO demo_user;

-- 2. Connect as demo_user and create the demo table
-- (run separately: CONNECT demo_user/demo_pass@//192.168.0.231:1521/ORCLPDB1)

CREATE TABLE demo_items (
  id   NUMBER PRIMARY KEY,
  name VARCHAR2(100 CHAR) NOT NULL
);

INSERT INTO demo_items (id, name) VALUES (1, 'First item from Oracle 19c');
INSERT INTO demo_items (id, name) VALUES (2, 'Second item from Oracle 19c');
INSERT INTO demo_items (id, name) VALUES (3, 'Third item from Oracle 19c');
COMMIT;
