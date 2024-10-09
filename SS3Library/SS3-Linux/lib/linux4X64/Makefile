# ----------------------------------------------------------------------------
# --- Installer for lib files                                     Makefile ---
# ---                                                                      ---
# ---  created:  19-Nob-2019 (der - SecuGen)
# ----------------------------------------------------------------------------
#

# --- library version ---
LIBSSLIB_VERSION_MAJOR = 1
LIBSSLIB_VERSION_MINOR = 0
LIBSSLIB_VERSION_MICRO = 0
LIBSSLIB_VERSION       = $(LIBSSLIB_VERSION_MAJOR).$(LIBSSLIB_VERSION_MINOR).$(LIBSSLIB_VERSION_MICRO)

# --- global definitions ---
SUDO         = sudo
LIB_LOCATION = /usr/local/lib

# --- definitions for compiling linux (use -pg in CFLAGS when using gprof) ---

LIBSSLIB = libsecusearchapi.so

info:
	-@echo "  make install    ->  install the shared library"

install:
	$(SUDO) cp $(LIBSSLIB).$(LIBSSLIB_VERSION) $(LIB_LOCATION)
	$(SUDO) cp $(LIBSSLIB).$(LIBSSLIB_VERSION) $(LIB_LOCATION)/$(LIBSSLIB)
	$(SUDO) /sbin/ldconfig $(LIB_LOCATION)

uninstall:
	$(SUDO) rm -f $(LIB_LOCATION)/$(LIBSSLIB)*

