package com.example.roombookmobile.models

data class BookingDto(
    val bookingId: Int = 0,
    val userId: Int,
    val roomId: Int,
    val startTime: String,
    val endTime: String,
    val purpose: String?,
    val isConfirmed: Boolean = false
)