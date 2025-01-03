cmake_minimum_required(VERSION 2.8.10 FATAL_ERROR)
###############################################################################################################
# This script will build and run the examples from a compressed file
# Execute from a command line:
#     ctest -S HDF4_Examples.cmake,HDF4Examples-0.1.1-Source -C Release -V -O test.log
###############################################################################################################

set(INSTALLDIR "C:/Program Files/HDF_Group/HDF/4.2.11")
set(CTEST_CMAKE_GENERATOR "Visual Studio 11 2012 Win64")
set(STATICLIBRARIES "NO")
set(CTEST_SOURCE_NAME ${CTEST_SCRIPT_ARG})
set(CTEST_DASHBOARD_ROOT ${CTEST_SCRIPT_DIRECTORY})
set(CTEST_BUILD_CONFIGURATION "Release")
#set(NO_MAC_FORTRAN "true")
#set(BUILD_OPTIONS "${BUILD_OPTIONS} -DHDF_BUILD_FORTRAN:BOOL=ON")
set(CTEST_USE_TAR_SOURCE "${CTEST_SCRIPT_ARG}")

###############################################################################################################
#     Adjust the following SET Commands as needed
###############################################################################################################
if(WIN32)
  if(STATICLIBRARIES)
    set(BUILD_OPTIONS "${BUILD_OPTIONS} -DUSE_SHARED_LIBS:BOOL=OFF")
  endif(STATICLIBRARIES)
  set(ENV{HDF4_DIR} "${INSTALLDIR}/cmake/hdf4")
  set(CTEST_BINARY_NAME ${CTEST_SOURCE_NAME}\\build)
  set(CTEST_SOURCE_DIRECTORY "${CTEST_DASHBOARD_ROOT}\\${CTEST_SOURCE_NAME}")
  set(CTEST_BINARY_DIRECTORY "${CTEST_DASHBOARD_ROOT}\\${CTEST_BINARY_NAME}")
else(WIN32)
  if(STATICLIBRARIES)
    set(BUILD_OPTIONS "${BUILD_OPTIONS} -DUSE_SHARED_LIBS:BOOL=OFF -DCMAKE_ANSI_CFLAGS:STRING=-fPIC")
  endif(STATICLIBRARIES)
  set(ENV{HDF4_DIR} "${INSTALLDIR}/share/cmake/hdf4")
  set(ENV{LD_LIBRARY_PATH} "${INSTALLDIR}/lib")
  set(CTEST_BINARY_NAME ${CTEST_SOURCE_NAME}/build)
  set(CTEST_SOURCE_DIRECTORY "${CTEST_DASHBOARD_ROOT}/${CTEST_SOURCE_NAME}")
  set(CTEST_BINARY_DIRECTORY "${CTEST_DASHBOARD_ROOT}/${CTEST_BINARY_NAME}")
endif(WIN32)

###############################################################################################################
# For any comments please contact cdashhelp@hdfgroup.org
#
###############################################################################################################
 
#-----------------------------------------------------------------------------
# MAC machines need special option
#-----------------------------------------------------------------------------
if(APPLE)
  # Compiler choice
  execute_process(COMMAND xcrun --find cc OUTPUT_VARIABLE XCODE_CC OUTPUT_STRIP_TRAILING_WHITESPACE)
  execute_process(COMMAND xcrun --find c++ OUTPUT_VARIABLE XCODE_CXX OUTPUT_STRIP_TRAILING_WHITESPACE)
  set(ENV{CC} "${XCODE_CC}")
  set(ENV{CXX} "${XCODE_CXX}")
  if(NOT NO_MAC_FORTRAN)
    # Shared fortran is not supported, build static 
    set(BUILD_OPTIONS "${BUILD_OPTIONS} -DBUILD_SHARED_LIBS:BOOL=OFF -DCMAKE_ANSI_CFLAGS:STRING=-fPIC")
  else(NOT NO_MAC_FORTRAN)
    set(BUILD_OPTIONS "${BUILD_OPTIONS} -DHDF_BUILD_FORTRAN:BOOL=OFF")
  endif(NOT NO_MAC_FORTRAN)
  set(BUILD_OPTIONS "${BUILD_OPTIONS} -DCTEST_USE_LAUNCHERS:BOOL=ON -DCMAKE_BUILD_WITH_INSTALL_RPATH:BOOL=OFF")
endif(APPLE)
 
#-----------------------------------------------------------------------------
set(CTEST_CMAKE_COMMAND "\"${CMAKE_COMMAND}\"")
## --------------------------
if(CTEST_USE_TAR_SOURCE)
  ## Uncompress source if tar or zip file provided
  ## --------------------------
  if(WIN32)
    message(STATUS "extracting... [${CMAKE_EXECUTABLE_NAME} -E tar -xvf ${CTEST_USE_TAR_SOURCE}.zip]")
    execute_process(COMMAND ${CMAKE_EXECUTABLE_NAME} -E tar -xvf ${CTEST_USE_TAR_SOURCE}.zip RESULT_VARIABLE rv)
  else(WIN32)
    message(STATUS "extracting... [${CMAKE_EXECUTABLE_NAME} -E tar -xvf ${CTEST_USE_TAR_SOURCE}.tar]")
    execute_process(COMMAND ${CMAKE_EXECUTABLE_NAME} -E tar -xvf ${CTEST_USE_TAR_SOURCE}.tar RESULT_VARIABLE rv)
  endif(WIN32)
 
  if(NOT rv EQUAL 0)
    message(STATUS "extracting... [error-(${rv}) clean up]")
    file(REMOVE_RECURSE "${CTEST_SOURCE_DIRECTORY}")
    message(FATAL_ERROR "error: extract of ${CTEST_SOURCE_NAME} failed")
  endif(NOT rv EQUAL 0)
endif(CTEST_USE_TAR_SOURCE)
 
#-----------------------------------------------------------------------------
## Clear the build directory
## --------------------------
set(CTEST_START_WITH_EMPTY_BINARY_DIRECTORY TRUE)
if (EXISTS "${CTEST_BINARY_DIRECTORY}" AND IS_DIRECTORY "${CTEST_BINARY_DIRECTORY}")
  ctest_empty_binary_directory(${CTEST_BINARY_DIRECTORY})
else (EXISTS "${CTEST_BINARY_DIRECTORY}" AND IS_DIRECTORY "${CTEST_BINARY_DIRECTORY}")
  file(MAKE_DIRECTORY "${CTEST_BINARY_DIRECTORY}")
endif (EXISTS "${CTEST_BINARY_DIRECTORY}" AND IS_DIRECTORY "${CTEST_BINARY_DIRECTORY}")

# Use multiple CPU cores to build
include(ProcessorCount)
ProcessorCount(N)
if(NOT N EQUAL 0)
  if(NOT WIN32)
    set(CTEST_BUILD_FLAGS -j${N})
  endif(NOT WIN32)
  set(ctest_test_args ${ctest_test_args} PARALLEL_LEVEL ${N})
endif()
set (CTEST_CONFIGURE_COMMAND
    "${CTEST_CMAKE_COMMAND} -C \"${CTEST_SOURCE_DIRECTORY}/config/cmake/cacheinit.cmake\" -DCMAKE_BUILD_TYPE:STRING=${CTEST_BUILD_CONFIGURATION} ${BUILD_OPTIONS} \"-G${CTEST_CMAKE_GENERATOR}\" \"${CTEST_SOURCE_DIRECTORY}\""
)
 
#-----------------------------------------------------------------------------
## -- set output to english
set($ENV{LC_MESSAGES}  "en_EN")
 
#-----------------------------------------------------------------------------
  ## NORMAL process
  ## --------------------------
  CTEST_START (Experimental)
  CTEST_CONFIGURE (BUILD "${CTEST_BINARY_DIRECTORY}")
  CTEST_READ_CUSTOM_FILES ("${CTEST_BINARY_DIRECTORY}")
  CTEST_BUILD (BUILD "${CTEST_BINARY_DIRECTORY}" APPEND)
  CTEST_TEST (BUILD "${CTEST_BINARY_DIRECTORY}" APPEND ${ctest_test_args} RETURN_VALUE res)
  if(res GREATER 0)
    message (FATAL_ERROR "tests FAILED")
  endif(res GREATER 0)
#-----------------------------------------------------------------------------
############################################################################################################## 
message(STATUS "DONE")
