CREATE DATABASE schoolmanagement;

USE schoolmanagement;

CREATE TABLE users(
id INT PRIMARY KEY IDENTITY(1,1),
username VARCHAR(20) UNIQUE,
psswd VARCHAR(40),
usertype CHAR(1),
userstatus VARCHAR(10),
mail VARCHAR(40) UNIQUE
);

CREATE TABLE students(
id INT PRIMARY KEY IDENTITY(1,1),
studentname VARCHAR(40),
courseyear INT,
idUser INT FOREIGN KEY REFERENCES users(id)
);

CREATE TABLE teachers(
id INT PRIMARY KEY IDENTITY(1,1),
teachername VARCHAR(40),
idUser INT FOREIGN KEY REFERENCES users(id)
);

CREATE TABLE courses(
id INT PRIMARY KEY IDENTITY(1,1),
coursename VARCHAR(40),
coursestatus VARCHAR(10),
courseyear INT
);

CREATE TABLE assignments(
id INT PRIMARY KEY IDENTITY(1,1),
assignmentname VARCHAR(20),
coursevalue INT,
assignmentstatus VARCHAR(10),
idCourse INT FOREIGN KEY REFERENCES courses(id)
);

CREATE TABLE teachersenrolleds(
id INT PRIMARY KEY IDENTITY(1,1),
enrolledstatus VARCHAR(10),
idTeacher INT,
idCourse INT FOREIGN KEY REFERENCES courses(id)
);

CREATE TABLE inscriptions(
id INT PRIMARY KEY IDENTITY(1,1),
generalgrade DECIMAL(5,2),
inscriptionstatus VARCHAR(10),
progress INT,
avarage DECIMAL(4,2),
idStudent INT FOREIGN KEY REFERENCES students(id),
idCourse INT FOREIGN KEY REFERENCES courses(id),
idTeacher INT
);

CREATE TABLE grades(
id INT PRIMARY KEY IDENTITY(1,1),
grade DECIMAL(4,2),
gradevalue DECIMAL(5,2),
idAssignment INT,
idInscription INT FOREIGN KEY REFERENCES inscriptions(id)
);