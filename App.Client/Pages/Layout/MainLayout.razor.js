﻿export function isMobile() {
    return /android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini|mobile/i.test(navigator.userAgent);
}

export function isDarkMode() {
    const prefersDarkMode = window.matchMedia("(prefers-color-scheme: dark)").matches;
    return prefersDarkMode;
}