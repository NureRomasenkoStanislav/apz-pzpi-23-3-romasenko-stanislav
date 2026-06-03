package com.example.roombookmobile.models

data class RoomDto(
    val roomId: Int,
    val name: String,
    val capacity: Int,
    val description: String?,
    val isArchived: Boolean,
    val isLocked: Boolean
)