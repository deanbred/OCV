#----------------------------------------------------------------
# Generated CMake target import file for configuration "Release".
#----------------------------------------------------------------

# Commands may need to know the format version.
set(CMAKE_IMPORT_FILE_VERSION 1)

# Import target "jpeg" for configuration "Release"
set_property(TARGET jpeg APPEND PROPERTY IMPORTED_CONFIGURATIONS RELEASE)
set_target_properties(jpeg PROPERTIES
  IMPORTED_IMPLIB_RELEASE "${_IMPORT_PREFIX}/lib/jpeg.lib"
  IMPORTED_LOCATION_RELEASE "${_IMPORT_PREFIX}/bin/jpeg.dll"
  )

list(APPEND _IMPORT_CHECK_TARGETS jpeg )
list(APPEND _IMPORT_CHECK_FILES_FOR_jpeg "${_IMPORT_PREFIX}/lib/jpeg.lib" "${_IMPORT_PREFIX}/bin/jpeg.dll" )

# Commands beyond this point should not need to know the version.
set(CMAKE_IMPORT_FILE_VERSION)
