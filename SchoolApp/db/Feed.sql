-- ============================================
-- Insert Roles
-- ============================================
INSERT INTO [dbo].[Roles] ([Name])
VALUES
    ('ADMIN'),
    ('EMPLOYEE'),
    ('TEACHER'),
    ('STUDENT');

-- ============================================
-- Insert Capabilities
-- ============================================
INSERT INTO [dbo].[Capabilities] ([Name], [Description])
VALUES
    ('INSERT_TEACHER', 'Create a new teacher'),
    ('VIEW_TEACHERS', 'View teacher list and details'),
    ('VIEW_TEACHER', 'View teacher'),
    ('EDIT_TEACHER', 'Modify existing teacher'),
    ('DELETE_TEACHER', 'Remove a teacher'),
    ('VIEW_ONLY_TEACHER', 'View only own teacher details'),
    ('INSERT_STUDENT', 'Create a new student'),
    ('VIEW_STUDENTS', 'View student list and details'),
    ('VIEW_STUDENT', 'View student'),
    ('EDIT_STUDENT', 'Modify existing student'),
    ('DELETE_STUDENT', 'Remove a student'),
    ('VIEW_ONLY_STUDENT', 'View only own student details'),
    ('INSERT_COURSE', 'Create a new course'),
    ('VIEW_COURSES', 'View course list and details'),
    ('VIEW_COURSE', 'View course'),
    ('EDIT_COURSE', 'Modify existing course'),
    ('DELETE_COURSE', 'Remove a course');


-- ============================================
-- ADMIN: all capabilities
-- ============================================
INSERT INTO [dbo].[RolesCapabilities] ([RolesId], [CapabilitiesId])
SELECT r.[Id], c.[Id]
FROM [dbo].[Roles] r
CROSS JOIN [dbo].[Capabilities] c
WHERE r.[Name] = 'ADMIN';


-- ============================================
-- EMPLOYEE: VIEW_TEACHERS, VIEW_TEACHER,
--           VIEW_STUDENTS, VIEW_STUDENT,
--           VIEW_COURSES, VIEW_COURSE
-- ============================================
INSERT INTO [dbo].[RolesCapabilities] ([RolesId], [CapabilitiesId])
SELECT r.[Id], c.[Id]
FROM [dbo].[Roles] r
CROSS JOIN [dbo].[Capabilities] c
WHERE r.[Name] = 'EMPLOYEE'
  AND c.[Name] IN ('VIEW_TEACHERS', 'VIEW_TEACHER',
                    'VIEW_STUDENTS', 'VIEW_STUDENT',
                    'VIEW_COURSES', 'VIEW_COURSE');


-- ============================================
-- TEACHER: VIEW_ONLY_TEACHER
-- ============================================
INSERT INTO [dbo].[RolesCapabilities] ([RolesId], [CapabilitiesId])
SELECT r.[Id], c.[Id]
FROM [dbo].[Roles] r
CROSS JOIN [dbo].[Capabilities] c
WHERE r.[Name] = 'TEACHER'
  AND c.[Name] IN ('VIEW_ONLY_TEACHER');


-- ============================================
-- STUDENT: VIEW_ONLY_STUDENT
-- ============================================
INSERT INTO [dbo].[RolesCapabilities] ([RolesId], [CapabilitiesId])
SELECT r.[Id], c.[Id]
FROM [dbo].[Roles] r
CROSS JOIN [dbo].[Capabilities] c
WHERE r.[Name] = 'STUDENT'
  AND c.[Name] IN ('VIEW_ONLY_STUDENT');

