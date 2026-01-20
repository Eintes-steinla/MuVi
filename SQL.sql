USE master
GO
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'MuVi')
    DROP DATABASE MuVi;
GO
CREATE DATABASE MuVi
GO
USE MuVi
GO

-- Bảng Quốc gia
CREATE TABLE Countries (
    CountryID INT PRIMARY KEY IDENTITY(1,1),
    CountryName NVARCHAR(100) NOT NULL UNIQUE,
    CountryCode NVARCHAR(10)
);

-- Bảng Thể loại
CREATE TABLE Genres (
    GenreID INT PRIMARY KEY IDENTITY(1,1),
    GenreName NVARCHAR(100) NOT NULL UNIQUE,
    [Description] NVARCHAR(500)
);

-- Bảng Diễn viên
CREATE TABLE Actors (
    ActorID INT PRIMARY KEY IDENTITY(1,1),
    ActorName NVARCHAR(100) NOT NULL,
    Bio NVARCHAR(MAX),
    PhotoPath NVARCHAR(500),
    DateOfBirth DATE,
    Nationality NVARCHAR(100),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- =============================================
-- BẢNG PHIM
-- =============================================

-- Bảng Phim chính
CREATE TABLE Movies (
    MovieID INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(255) NOT NULL, -- Tên phim
    MovieType NVARCHAR(20) NOT NULL CHECK (MovieType IN (N'Phim lẻ', N'Phim bộ')),
    CountryID INT,
    Director NVARCHAR(100), -- Tác giả
    ReleaseYear INT, -- Năm phát hành
    [Description] NVARCHAR(MAX),
    PosterPath NVARCHAR(500),
    TrailerURL NVARCHAR(500),
    Duration INT, -- Thời lượng (phút) - dùng cho phim lẻ
    TotalEpisodes INT, -- Tổng số tập - dùng cho phim bộ
    Rating DECIMAL(3,2) DEFAULT 0.00 CHECK (Rating BETWEEN 0 AND 10), -- Đánh giá trung bình
    ViewCount INT DEFAULT 0, -- Lượt xem
    [Status] NVARCHAR(20) DEFAULT N'Đang chiếu' CHECK ([Status] IN (N'Đang chiếu', N'Hoàn thành', N'Sắp chiếu')),
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Movies_Countries FOREIGN KEY (CountryID) 
        REFERENCES Countries(CountryID) ON DELETE SET NULL
);

-- Bảng Tập phim (cho phim bộ)
CREATE TABLE Episodes (
    EpisodeID INT PRIMARY KEY IDENTITY(1,1),
    MovieID INT NOT NULL,
    EpisodeNumber INT NOT NULL,
    Title NVARCHAR(255),
    [Description] NVARCHAR(MAX),
    Duration INT, -- Thời lượng tập (phút)
    VideoPath NVARCHAR(500),
    ReleaseDate DATE,
    ViewCount INT DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Episodes_Movies FOREIGN KEY (MovieID) 
        REFERENCES Movies(MovieID) ON DELETE CASCADE,
    CONSTRAINT UQ_Episode UNIQUE (MovieID, EpisodeNumber)
);

-- =============================================
-- BẢNG QUAN HỆ NHIỀU-NHIỀU
-- =============================================

-- Bảng trung gian: Phim - Thể loại
CREATE TABLE MovieCategory (
    MovieID INT,
    GenreID INT,
    PRIMARY KEY (MovieID, GenreID),
    CONSTRAINT FK_MC_Movies FOREIGN KEY (MovieID) 
        REFERENCES Movies(MovieID) ON DELETE CASCADE,
    CONSTRAINT FK_MC_Genres FOREIGN KEY (GenreID) 
        REFERENCES Genres(GenreID) ON DELETE CASCADE
);

-- Bảng trung gian: Phim - Diễn viên
CREATE TABLE MovieCast (
    MovieID INT,
    ActorID INT,
    RoleName NVARCHAR(100), -- Vai diễn
    [Order] INT DEFAULT 0, -- Thứ tự xuất hiện (diễn viên chính = 1,2,3...)
    PRIMARY KEY (MovieID, ActorID),
    CONSTRAINT FK_Cast_Movies FOREIGN KEY (MovieID) 
        REFERENCES Movies(MovieID) ON DELETE CASCADE,
    CONSTRAINT FK_Cast_Actors FOREIGN KEY (ActorID) 
        REFERENCES Actors(ActorID) ON DELETE CASCADE
);

-- =============================================
-- BẢNG NGƯỜI DÙNG
-- =============================================

-- Bảng Users
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    [Password] NVARCHAR(255) NOT NULL, 
    Email NVARCHAR(100) UNIQUE,
    DateOfBirth DATE,
    Avatar NVARCHAR(500),
    [Role] NVARCHAR(20) DEFAULT 'User' CHECK ([Role] IN ('Admin', 'User')),
    IsActive BIT DEFAULT 1,
    LastLogin DATETIME,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE()
);

-- =============================================
-- BẢNG TƯƠNG TÁC NGƯỜI DÙNG
-- =============================================

-- Bảng Đánh giá & Bình luận
CREATE TABLE Reviews (
    ReviewID INT PRIMARY KEY IDENTITY(1,1),
    MovieID INT NOT NULL,
    UserID INT NOT NULL,
    Rating INT CHECK (Rating BETWEEN 1 AND 10),
    Comment NVARCHAR(MAX),
    LikeCount INT DEFAULT 0, -- đếm số lượt thích (like) mà bình luận/review nhận được từ người dùng khác
    CreatedAt DATETIME DEFAULT GETDATE(),
    UpdatedAt DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Reviews_Movies FOREIGN KEY (MovieID) 
        REFERENCES Movies(MovieID) ON DELETE CASCADE,
    CONSTRAINT FK_Reviews_Users FOREIGN KEY (UserID) 
        REFERENCES Users(UserID) ON DELETE CASCADE,
    CONSTRAINT UQ_UserMovie_Review UNIQUE (UserID, MovieID) -- Mỗi user chỉ review 1 lần/phim
);

-- Bảng Danh sách yêu thích
CREATE TABLE Favorites (
    UserID INT,
    MovieID INT,
    AddedAt DATETIME DEFAULT GETDATE(),
    PRIMARY KEY (UserID, MovieID),
    CONSTRAINT FK_Fav_Users FOREIGN KEY (UserID) 
        REFERENCES Users(UserID) ON DELETE CASCADE,
    CONSTRAINT FK_Fav_Movies FOREIGN KEY (MovieID) 
        REFERENCES Movies(MovieID) ON DELETE CASCADE
);

-- Bảng Lịch sử xem phim
CREATE TABLE ViewHistory (
    HistoryID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT NOT NULL,
    MovieID INT NOT NULL,
    EpisodeID INT, -- NULL nếu là phim lẻ
    WatchedAt DATETIME DEFAULT GETDATE(),
    WatchDuration INT DEFAULT 0, -- Thời gian đã xem (giây)
    IsCompleted BIT DEFAULT 0, -- Đã xem hết chưa
    CONSTRAINT FK_History_Users FOREIGN KEY (UserID) 
        REFERENCES Users(UserID) ON DELETE CASCADE,
    CONSTRAINT FK_History_Movies FOREIGN KEY (MovieID) 
        REFERENCES Movies(MovieID) ON DELETE CASCADE,
    CONSTRAINT FK_History_Episodes FOREIGN KEY (EpisodeID) 
        REFERENCES Episodes(EpisodeID) ON DELETE NO ACTION
);

-- =============================================
-- INDEXES ĐỂ TỐI ƯU HIỆU SUẤT
-- =============================================

-- Index cho tìm kiếm phim
CREATE INDEX IX_Movies_Title ON Movies(Title);
CREATE INDEX IX_Movies_Status ON Movies([Status]);
CREATE INDEX IX_Movies_Rating ON Movies(Rating DESC);
CREATE INDEX IX_Movies_ViewCount ON Movies(ViewCount DESC);

-- Index cho Episodes
CREATE INDEX IX_Episodes_MovieID ON Episodes(MovieID);

-- Index cho Reviews
CREATE INDEX IX_Reviews_MovieID ON Reviews(MovieID);
CREATE INDEX IX_Reviews_UserID ON Reviews(UserID);

-- Index cho ViewHistory
CREATE INDEX IX_ViewHistory_UserID ON ViewHistory(UserID);
CREATE INDEX IX_ViewHistory_MovieID ON ViewHistory(MovieID);

-- Index cho Users
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_Username ON Users(Username);

GO

-- =============================================
-- DỮ LIỆU MẪU
-- =============================================

-- Thêm Countries
INSERT INTO Countries (CountryName, CountryCode) VALUES
(N'Việt Nam', 'VN'),
(N'Hàn Quốc', 'KR'),
(N'Nhật Bản', 'JP'),
(N'Mỹ', 'US'),
(N'Trung Quốc', 'CN'),
(N'Thái Lan', 'TH');

-- Thêm Genres
INSERT INTO Genres (GenreName, [Description]) VALUES
(N'Hành động', N'Phim hành động, võ thuật'),
(N'Tình cảm', N'Phim tình cảm, lãng mạn'),
(N'Hài', N'Phim hài hước'),
(N'Kinh dị', N'Phim kinh dị, ma'),
(N'Khoa học viễn tưởng', N'Phim sci-fi'),
(N'Hoạt hình', N'Phim hoạt hình, anime'),
(N'Tâm lý', N'Phim tâm lý, drama'),
(N'Trinh thám', N'Phim trinh thám, bí ẩn');

-- Thêm Actors mẫu
INSERT INTO Actors (ActorName, Nationality, DateOfBirth) VALUES
(N'Trấn Thành', N'Việt Nam', '1987-02-05'),
(N'Ninh Dương Lan Ngọc', N'Việt Nam', '1990-06-25'),
(N'Lee Min Ho', N'Hàn Quốc', '1987-06-22'),
(N'Son Ye Jin', N'Hàn Quốc', '1982-01-11');

-- Thêm Users mẫu
INSERT INTO Users (Username, [Password], FullName, Email, [Role]) VALUES
('admin', 'admin123', N'Quản trị viên', 'admin@muvi.com', 'Admin'),
('user1', 'user123', N'Nguyễn Văn A', 'user1@gmail.com', 'User');

GO

PRINT N'✅ Tạo database MuVi thành công!'
PRINT N'📊 Bảng đã tạo:'
PRINT N'   - Countries, Genres, Actors'
PRINT N'   - Movies, Episodes'
PRINT N'   - MovieCategory, MovieCast'
PRINT N'   - Users, Reviews, Favorites, ViewHistory'
PRINT N'🔍 Indexes đã được tạo để tối ưu hiệu suất'
PRINT N'📝 Dữ liệu mẫu đã được thêm vào'