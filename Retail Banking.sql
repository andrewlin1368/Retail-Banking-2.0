create table Customer(
CustomerID int primary key identity(0,1),
SSNID int unique,
Age int,
Name varchar(50),
Address varchar(50),
Email varchar(100) unique,
Status varchar(50)) 


create table Account(
CustomerID int references Customer(CustomerID),
AccountID int primary key identity(1,1),
AccountType varchar(10),
AccountBalance decimal,
Status varchar(150),
constraint chk_AccountType Check (AccountType in ('Checking','Saving')))


insert into Account values(0,'Checking',0,'System')
insert into Account values(0,'Saving',0,'System')

create table Transactions(
TransactionID int primary key identity(1,1),
Date DateTime,
Type varchar(150),
Amount decimal,
FromAccountID int,
ToAccountID int)

create table Error(
ErrorID int unique,
ErrorMessage varchar(100))

insert into Error values(1,'Customer exists.')
insert into Error values(2,'Customer does not exists.')
insert into Error values(100,'Checking and Saving Account exists.')
insert into Error values(101,'Checking account exists.')
insert into Error values(102,'Saving account exists.')
insert into Error values(103,'No accounts found.')
insert into Error values(104,'Checking and/or Saving Account exists.')
insert into Error values(105,'No Customers found.')
insert into Error values(500,'No transactions found.')
insert into Error values(600,'Insufficent funds.')
insert into Error values(601,'Balance must be atleast 0.')
insert into Error values(602,'Amount must be greater than 0.')
insert into Error values(603,'Account has a balance.')
insert into Error values(701,'Customer ID not found.')
insert into Error values(702,'Account ID not found.')
insert into Error values(703,'SSN ID not found.')
insert into Error values(704,'SSN ID or Customer ID must be provided.')
insert into Error values(705,'Account ID cannot be the same.')
insert into Error values(800,'No transactions found.')
insert into Error values(900,'To Account ID or Amount is invalid.')
insert into Error values(1000,'Default Data.')

insert into Customer values (0,0,'System','System','UserManager@UserManager.com','System')

select * from Error

select * from Customer

select * from Account

select * from Transactions
