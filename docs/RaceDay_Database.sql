create database RaceDayDB;
go

USE RaceDayDB;
go
-- Create Organiser Table
CREATE TABLE Organiser (
    OrganiserID INT PRIMARY KEY IDENTITY(1,1),
    OrganiserName VARCHAR(100) NOT NULL,
    OrganiserEmail VARCHAR(100) NOT NULL UNIQUE,
    OrganiserPassword VARCHAR(255) NOT NULL
    );

-- Create Event Table
 CREATE TABLE Event (
    EventID INT PRIMARY KEY IDENTITY(1,1),
    EventName VARCHAR(100) NOT NULL,
    EventDate DATE NOT NULL,
    EventLocation VARCHAR(100) NOT NULL,
    EventDescription VARCHAR(500) NOT NULL,
    OrganiserID INT NOT NULL,
    FOREIGN KEY (OrganiserID) REFERENCES Organiser(OrganiserID)
    );

-- Create Category Table
 CREATE TABLE Category (
    CategoryID INT PRIMARY KEY IDENTITY (1,1),
    CategoryName VARCHAR(100) NOT NULL,
    EventID INT NOT NULL,
    FOREIGN KEY (EventID) REFERENCES Event(EventID)
    );

-- Create Participant Table
  CREATE TABLE Participant(
    ParticipantID INT PRIMARY KEY IDENTITY(1,1),
    ParticipantName VARCHAR(100) NOT NULL,
    ParticipantEmail VARCHAR(100) NOT NULL UNIQUE,
    ParticipantPassword VARCHAR (255) NOT NULL
    );

-- Create Enrolment Table
 CREATE TABLE Enrolment(
    EnrolmentID INT PRIMARY KEY IDENTITY (1,1),
    ParticipantID INT NOT NULL,
    CategoryID INT NOT NULL,
    FOREIGN KEY (ParticipantID) REFERENCES Participant(ParticipantID),
    FOREIGN KEY (CategoryID) REFERENCES Category(CategoryID)
    );

-- Create Result Table
 CREATE TABLE Result(
    ResultID INT PRIMARY KEY IDENTITY (1,1),
    FinishTime TIME NOT NULL,
    Position INT NOT NULL,
    EnrolmentID INT NOT NULL,
    FOREIGN KEY (EnrolmentID) REFERENCES Enrolment(EnrolmentID)
    );
  
  -- Insert Organisers
 INSERT INTO Organiser (OrganiserName,OrganiserEmail,OrganiserPassword)
 VALUES
 ('Jaden Smith', 'smith5512@gmail.com', 'smithman156'),
 ('Tommy Brown', 'tommybrown2569@mail.com', 'tombrown88825');

 -- Insert Participants
 INSERT INTO Participant (ParticipantName, ParticipantEmail,ParticipantPassword)
 VALUES
 ('James St Patrick', 'patrickjames@gmail.com', 'james12hunderd'),
 ('Tariq Egan', 'eganholds@gmail.com', 'tomany226');

 -- Insert Events
 INSERT INTO Event (EventName,EventDate,EventLocation,EventDescription,OrganiserID)
 Values
 ('Cape Town City Run', '2020-07-10', 'Cape Town', 'An amazing 10km road race through the city', 1),
 ('Table Mountain Trail Challenge', '2026-08-02', 'Table Mountain', 'A challanging trail run mixed with the amazing views', 1),
 ('Winelands Cycle Tour', '2026-09-06', 'Stellenbosch', 'A cycling event through the amazing Cape Winelands', 2);

 -- Insert Categories
 INSERT INTO Category (CategoryName, EventID)
 VALUES
 ('15km', 1),
 ('25km', 1),
 ('7km', 2),
 ('Under 20', 3);

 -- Insert Enrolment 
 INSERT INTO Enrolment (ParticipantID, CategoryID)
 VALUES
 (1, 1),
 (2, 3);

 -- Insert Result 
 INSERT INTO Result (FinishTime, Position, EnrolmentID)
 VALUES
 ('00:45:30', 1, 1),
 ('00:56:10', 2, 2);


