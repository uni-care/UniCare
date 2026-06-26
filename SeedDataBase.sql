DECLARE @OwnerId UNIQUEIDENTIFIER = '949941D8-9483-4FAE-6F95-08DEC5B305FB';
DECLARE @CatArt UNIQUEIDENTIFIER = '44444444-4444-4444-4444-444444444444';
DECLARE @CatDorm UNIQUEIDENTIFIER = 'AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA';
DECLARE @CatElectronics UNIQUEIDENTIFIER = '77777777-7777-7777-7777-777777777777';

INSERT INTO Items
    (Id, Title, Description, Price, Status, OwnerId, CreatedAt, UpdatedAt, AvailableFrom, AvailableTo, CategoryId, ImageUrls, Location)
VALUES
(NEWID(), N'Scientific Calculator Casio FX-991', N'Casio FX-991ES Plus, barely used, perfect for engineering and math courses.', '350.00', 0, @OwnerId, SYSUTCDATETIME(), SYSUTCDATETIME(), SYSUTCDATETIME(), DATEADD(MONTH, 3, SYSUTCDATETIME()), @CatElectronics, N'["https://example.com/img/calc1.jpg"]', N'Qena'),

(NEWID(), N'Graphic Drawing Tablet - Wacom Intuos', N'Used for digital art assignments, comes with pen and USB cable.', '1200.00', 0, @OwnerId, SYSUTCDATETIME(), SYSUTCDATETIME(), SYSUTCDATETIME(), DATEADD(MONTH, 2, SYSUTCDATETIME()), @CatElectronics, N'["https://example.com/img/tablet1.jpg"]', N'Qena'),

(NEWID(), N'Laptop Stand - Adjustable Aluminum', N'Great for long study sessions, foldable and lightweight.', '250.00', 0, @OwnerId, SYSUTCDATETIME(), SYSUTCDATETIME(), NULL, NULL, @CatElectronics, N'["https://example.com/img/standlaptop.jpg"]', N'Qena'),

(NEWID(), N'USB Microscope Camera', N'For biology lab sessions, connects to laptop via USB.', '480.00', 1, @OwnerId, SYSUTCDATETIME(), SYSUTCDATETIME(), NULL, NULL, @CatElectronics, N'["https://example.com/img/microcam.jpg"]', N'Qena'),

(NEWID(), N'Engineering Drafting Set', N'Complete drafting set with compass, rulers, and protractor for technical drawing.', '180.00', 0, @OwnerId, SYSUTCDATETIME(), SYSUTCDATETIME(), NULL, NULL, @CatArt, N'["https://example.com/img/drafting1.jpg"]', N'Qena'),

(NEWID(), N'Acrylic Paint Set - 24 Colors', N'Used once for an art elective project, almost full tubes.', '220.00', 0, @OwnerId, SYSUTCDATETIME(), SYSUTCDATETIME(), NULL, NULL, @CatArt, N'["https://example.com/img/paintset.jpg"]', N'Qena'),

(NEWID(), N'Architecture Model Making Kit', N'Foam board, cutting mat, and scale rulers for architecture studio projects.', '300.00', 0, @OwnerId, SYSUTCDATETIME(), SYSUTCDATETIME(), NULL, NULL, @CatArt, N'["https://example.com/img/modelkit.jpg"]', N'Qena'),

(NEWID(), N'Calligraphy Pen Set', N'For graphic design and typography coursework, gently used.', '150.00', 0, @OwnerId, SYSUTCDATETIME(), SYSUTCDATETIME(), NULL, NULL, @CatArt, N'["https://example.com/img/calligraphy.jpg"]', N'Qena'),

(NEWID(), N'Anatomy Lab Coat - Medium', N'White lab coat for medical/anatomy practicals, washed and ready.', '120.00', 0, @OwnerId, SYSUTCDATETIME(), SYSUTCDATETIME(), NULL, NULL, @CatDorm, N'["https://example.com/img/labcoat.jpg"]', N'Qena'),

(NEWID(), N'Desk Lamp - LED Study Light', N'Adjustable brightness, perfect for late-night studying in dorm rooms.', '140.00', 0, @OwnerId, SYSUTCDATETIME(), SYSUTCDATETIME(), NULL, NULL, @CatDorm, N'["https://example.com/img/desklamp.jpg"]', N'Qena'),

(NEWID(), N'Mini Fridge for Dorm Room', N'Compact fridge, great for keeping snacks/drinks cold during study sessions.', '1500.00', 2, @OwnerId, SYSUTCDATETIME(), SYSUTCDATETIME(), NULL, NULL, @CatDorm, N'["https://example.com/img/minifridge.jpg"]', N'Qena'),

(NEWID(), N'Bookshelf - 5 Tier Wooden', N'Sturdy wooden bookshelf, ideal for organizing textbooks and binders.', '600.00', 0, @OwnerId, SYSUTCDATETIME(), SYSUTCDATETIME(), NULL, NULL, @CatDorm, N'["https://example.com/img/bookshelf.jpg"]', N'Qena'),

(NEWID(), N'Organic Chemistry Textbook - 5th Edition', N'Slightly worn but all pages intact, used for one semester.', '280.00', 0, @OwnerId, SYSUTCDATETIME(), SYSUTCDATETIME(), NULL, NULL, @CatDorm, N'["https://example.com/img/chemistrybook.jpg"]', N'Qena'),

(NEWID(), N'Calculus Early Transcendentals Textbook', N'Standard calc textbook for engineering/science majors, minimal highlighting.', '300.00', 0, @OwnerId, SYSUTCDATETIME(), SYSUTCDATETIME(), NULL, NULL, @CatDorm, N'["https://example.com/img/calcbook.jpg"]', N'Qena'),

(NEWID(), N'Digital Multimeter for Electronics Lab', N'Used in physics/electronics labs, includes probes and manual.', '350.00', 0, @OwnerId, SYSUTCDATETIME(), SYSUTCDATETIME(), NULL, NULL, @CatElectronics, N'["https://example.com/img/multimeter.jpg"]', N'Qena'),

(NEWID(), N'Noise Cancelling Headphones', N'Great for studying in noisy dorm environments, good battery life.', '900.00', 0, @OwnerId, SYSUTCDATETIME(), SYSUTCDATETIME(), NULL, NULL, @CatElectronics, N'["https://example.com/img/headphones.jpg"]', N'Qena'),

(NEWID(), N'Whiteboard - Small Portable', N'Great for group study sessions and brainstorming, comes with markers.', '170.00', 0, @OwnerId, SYSUTCDATETIME(), SYSUTCDATETIME(), NULL, NULL, @CatDorm, N'["https://example.com/img/whiteboard.jpg"]', N'Qena'),

(NEWID(), N'Watercolor Paper Pad A3', N'Unused, 300gsm, ideal for fine arts and architecture rendering classes.', '90.00', 0, @OwnerId, SYSUTCDATETIME(), SYSUTCDATETIME(), NULL, NULL, @CatArt, N'["https://example.com/img/watercolorpad.jpg"]', N'Qena'),

(NEWID(), N'Physics Lab Manual with Solutions', N'Includes worked-out solutions for common physics lab experiments.', '110.00', 1, @OwnerId, SYSUTCDATETIME(), SYSUTCDATETIME(), NULL, NULL, @CatDorm, N'["https://example.com/img/physicsmanual.jpg"]', N'Qena'),

(NEWID(), N'Mechanical Pencil Set - Drafting Grade', N'Set of 5 mechanical pencils with various lead sizes for technical drawing.', '95.00', 0, @OwnerId, SYSUTCDATETIME(), SYSUTCDATETIME(), NULL, NULL, @CatArt, N'["https://example.com/img/pencilset.jpg"]', N'Qena'),

(NEWID(), N'External SSD 1TB for Backups', N'Used to back up coursework and design files, like new condition.', '1400.00', 0, @OwnerId, SYSUTCDATETIME(), SYSUTCDATETIME(), NULL, NULL, @CatElectronics, N'["https://example.com/img/ssd1tb.jpg"]', N'Qena');