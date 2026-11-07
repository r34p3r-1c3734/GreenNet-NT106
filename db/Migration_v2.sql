-- Chạy file này nếu bạn ĐÃ tạo database bằng Schema.sql của phiên bản trước (chưa có cột Protocol).
USE GreenNet;
GO
IF COL_LENGTH('Sessions', 'Protocol') IS NULL
    ALTER TABLE Sessions ADD Protocol VARCHAR(3) NOT NULL CONSTRAINT DF_Sessions_Protocol DEFAULT 'TCP';
GO
