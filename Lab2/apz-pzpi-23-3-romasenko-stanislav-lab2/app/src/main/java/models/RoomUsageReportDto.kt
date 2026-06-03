package com.example.roombookmobile.models

data class RoomUsageReportDto(
    val roomId: Int,
    val roomName: String,
    val totalBookedHours: Double,
    val usagePercentage: Double
)