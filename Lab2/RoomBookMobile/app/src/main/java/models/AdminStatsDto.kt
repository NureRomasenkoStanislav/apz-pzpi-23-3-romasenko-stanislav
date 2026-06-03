package com.example.roombookmobile.models

data class AdminStatsDto(
    val totalBookings: Int,
    val activeUsers: Int,
    val topRoomId: Int
)